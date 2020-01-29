﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using FurCoNZ.Web.DAL;
using FurCoNZ.Web.Helpers;
using FurCoNZ.Web.Models;

namespace FurCoNZ.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly FurCoNZDbContext _db;
        private readonly IEmailService _emailService;
        private readonly ILogger _logger;

        public OrderService(FurCoNZDbContext db, IEmailService emailService, ILogger<OrderService> logger)
        {
            _db = db;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Order> CreateOrderAsync(User purchasingAccount, IEnumerable<Ticket> ticketsInBasket, bool allowOrderingHiddenTickets = false, CancellationToken cancellationToken = default)
        {
            if (purchasingAccount == null)
                throw new ArgumentNullException(nameof(purchasingAccount), "Must supply a user when creating an order");
            if (ticketsInBasket == null || !ticketsInBasket.Any())
                throw new ArgumentNullException(nameof(ticketsInBasket), "No tickets are being purchased in the creation of this order");
            if (ticketsInBasket.Any(t => t.Id != default))
                throw new InvalidOperationException("Some of the tickets in this order already exist, and have been assigned an id.");

            // Setup
            var ticketList = ticketsInBasket.ToList();
            var ticketTypeIdsInOrder = ticketList.Select(t => t.TicketTypeId).Distinct();
            var ticketTypesInOrder = _db.TicketTypes.Where(tt => ticketTypeIdsInOrder.Contains(tt.Id));

            // Check tickets are still available
            foreach (var ticketType in ticketTypesInOrder)
            {
                var ticketsOfTypeAvailable = ticketType.TotalAvailable;
                var ticketsOfTypeOrdered = ticketList.Count(t => t.TicketTypeId == ticketType.Id);

                if (!allowOrderingHiddenTickets && ticketType.HiddenFromPublic)
                {
                    throw new InvalidOperationException($"{ticketType.Name} tickets require special permission to be ordered.");
                }

                if (ticketsOfTypeOrdered > ticketsOfTypeAvailable)
                {
                    throw new InvalidOperationException($"There are not enough {ticketType.Name} tickets available for this order to be created.");
                }

                if (ticketType.SoldOutAt <= DateTimeOffset.Now)
                {
                    throw new InvalidOperationException($"The cut-off date for {ticketType.Name} ticket has passed. They are no longer available for purchase.");
                }

                // Remove the appropriate number of tickets from the available pool
                ticketType.TotalAvailable -= ticketsOfTypeOrdered;
            }

            // Set up tickets for tracking
            _db.Tickets.AddRange(ticketList);

            // Set up order for tracking
            var order = new Order
            {
                OrderedById = purchasingAccount.Id,
                CreatedAt = DateTimeOffset.Now,
                TicketsPurchased = ticketList,
            };
            _db.Orders.Add(order);

            // Commit to DB
            await _db.SaveChangesAsync(cancellationToken);

            await _emailService.SendOrderConfirmationAsync(order, cancellationToken);

            return order;
        }

        public async Task<IEnumerable<TicketType>> GetTicketTypesAsync(bool includeUnavailableTickets = true, bool includeHiddenTickets = false, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var query = _db.TicketTypes.AsQueryable();

            if (!includeUnavailableTickets)
            {
                query = query.Where(tt => tt.TotalAvailable > 0 && tt.SoldOutAt > DateTimeOffset.Now);
            }

            if (!includeHiddenTickets)
            {
                query = query.Where(tt => !tt.HiddenFromPublic);
            }

            return await query.OrderByDescending(tt => tt.PriceCents).ToListAsync(cancellationToken);
        }

        public Task<DateTimeOffset> ReserveTicketsForPurchaseAsync(IDictionary<int, int> ticketsToReserveById, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(User user, CancellationToken cancellationToken = default)
        {
            // TODO: Should we support pagination?
            return await _db.Orders
                .Include(o => o.TicketsPurchased)
                .ThenInclude(t => t.TicketType)
                .Include(o => o.Audits)
                .Where(o => o.OrderedBy == user)
                .ToListAsync(cancellationToken);
        }

        public async Task<Order> GetUserOrderAsync(User user, int orderId, CancellationToken cancellationToken = default)
        {
            return await _db.Orders
                .Include(o => o.TicketsPurchased)
                .ThenInclude(t => t.TicketType)
                .Include(o => o.Audits)
                .SingleOrDefaultAsync(o => o.OrderedById == user.Id && o.Id == orderId, cancellationToken);
        }

        public async Task<Order> GetUserPendingOrderAsync(User user, CancellationToken cancellationToken = default)
        {
            return await _db.Orders
                .Include(o => o.TicketsPurchased)
                .ThenInclude(t => t.TicketType)
                .SingleOrDefaultAsync(o => o.OrderedById == user.Id && o.AmountPaidCents == 0, cancellationToken);
        }

        public async Task AddReceivedFundsForOrderAsync(int orderId, int amountCents, string paymentProvider, string paymentReference, DateTimeOffset when, bool allowDuplicates = false, CancellationToken cancellationToken = default)
        {
            var order = await _db.Orders
                .Include(o => o.Audits)
                .Include(o => o.OrderedBy)
                .Include(o => o.TicketsPurchased)
                .ThenInclude(t => t.TicketType)
                .SingleAsync(o => o.Id == orderId, cancellationToken);

            if (!allowDuplicates && order.Audits.Any(a => a.PaymentProvider == paymentProvider && a.PaymentProviderReference == paymentReference && a.Type == AuditType.Received))
            {
                // What about back transfers?
                _logger.LogError($"Received funds for order {orderId} has already been applied for {paymentProvider}: {paymentReference}");
            }
            else if (order.IsCancelled)
            {
                _logger.LogError($"Received funds for order {orderId} has already been cancelled.");
            }
            else
            {
                var audit = new OrderAudit
                {
                    OrderId = orderId,
                    PaymentProvider = paymentProvider,
                    PaymentProviderReference = paymentReference,
                    Type = AuditType.Received,
                    When = when,
                    AmountCents = amountCents,
                };

                // Recalculate amount paid
                order.AmountPaidCents = order.Audits.Sum(a => a.AmountCents) + audit.AmountCents;

                await _db.OrderAudits.AddAsync(audit, cancellationToken);
                await _db.SaveChangesAsync(cancellationToken);
            }

            if (order.AmountPaidCents >= order.TotalAmountCents)
            {
                await _emailService.SendOrderPaidAsync(order, cancellationToken);
            }
            else
            {
                await _emailService.SendPaymentReceivedAsync(order, cancellationToken);
            }

        }

        public async Task RefundFundsForOrderAsync(int orderId, int amountCents, string paymentProvider, string paymentReference, DateTimeOffset when, CancellationToken cancellationToken = default)
        {
            var order = await _db.Orders
                .Include(o => o.Audits)
                .Include(o => o.OrderedBy)
                .Include(o => o.TicketsPurchased)
                .ThenInclude(t => t.TicketType)
                .SingleAsync(o => o.Id == orderId, cancellationToken);

            if(order.Audits.Any(a => a.PaymentProvider == paymentProvider && a.PaymentProviderReference == paymentReference && a.Type == AuditType.Refunded))
            {
                _logger.LogError($"Refund for order {orderId} has already been applied for {paymentProvider}: {paymentReference}");
            }
            else
            {
                var audit = new OrderAudit
                {
                    OrderId = orderId,
                    PaymentProvider = paymentProvider,
                    PaymentProviderReference = paymentReference,
                    Type = AuditType.Refunded,
                    When = when,
                    AmountCents = -amountCents,
                };
                // Recalculate amount paid
                order.AmountPaidCents = order.Audits.Sum(a => a.AmountCents) + audit.AmountCents;
                
                _db.OrderAudits.Add(audit);

                await _db.SaveChangesAsync(cancellationToken);
            }

            await _emailService.SendPaymentRefundedAsync(order, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Should we support pagination?
            return await _db.Orders
                .Include(o => o.TicketsPurchased)
                .ThenInclude(t => t.TicketType)
                .Include(o => o.OrderedBy)
                .Include(o => o.Audits)
                .ToListAsync(cancellationToken);
        }

        public async Task<Order> GetOrderById(int orderId, CancellationToken cancellationToken = default)
        {
            return await _db.Orders
                .Include(o => o.TicketsPurchased)
                .ThenInclude(t => t.TicketType)
                .Include(o => o.Audits)
                .Include(o => o.OrderedBy)
                .SingleOrDefaultAsync(o => o.Id == orderId, cancellationToken);
        }

        public async Task<Order> GetOrderByRef(int orderRef, CancellationToken cancellationToken = default)
        {
            // Use Damm algorithum to verify order reference for mistyped value.
            if (!DammAlgorithm.IsValid(orderRef))
                throw new ArgumentOutOfRangeException("Invalid checksum value", nameof(orderRef));

            // Strip the check value from our orderRef to get the orderId
            return await GetOrderById(orderRef / 10, cancellationToken);
        }

        public async Task<IEnumerable<Ticket>> GetDetailedAttendeeListAsync(bool includeExpiredOrders = false, CancellationToken cancellationToken = default)
        {
            var ticketsListQuery = _db.Tickets.AsQueryable();

            if (!includeExpiredOrders)
            {
                ticketsListQuery = ticketsListQuery.Where(t => t.Order.AmountPaidCents > 0 || t.Order.TicketsPurchased.All(tp => tp.TicketType.SoldOutAt <= DateTimeOffset.Now));
            }

            return await ticketsListQuery
                .Include(t => t.TicketType)
                .Include(t => t.Order)
                .ThenInclude(o => o.OrderedBy)
                .Include(t => t.Order)
                .ThenInclude(o => o.Audits)
                .OrderBy(t => t.OrderId)
                .ToListAsync(cancellationToken);
        }

        public async Task CancelOrderAsync(int orderId, CancellationToken cancellationToken = default)
        {
            var order = await _db.Orders
                .Include(o => o.Audits)
                .Include(o => o.OrderedBy)
                .SingleAsync(o => o.Id == orderId, cancellationToken);

            if (order.Audits.Any(a => a.Type == AuditType.Received))
            {
                throw new InvalidOperationException("Can not cancel an order with received funds. Please refund it instead.");
            }

            var audit = new OrderAudit
            {
                OrderId = orderId,
                PaymentProvider = string.Empty,
                PaymentProviderReference = string.Empty,
                Type = AuditType.Cancelled,
                When = DateTimeOffset.Now,
                AmountCents = 0,
            };

            _db.OrderAudits.Add(audit);

            await _db.SaveChangesAsync(cancellationToken);

            // TODO: Send cancellation email.
            //await _emailService.SendOrderCancelledAsync(order, cancellationToken);
        }

        public async Task CheckInTicketAsync(int ticketId, CancellationToken cancellationToken)
        {
            var ticket = await _db.Tickets
                .Include(t => t.Order)
                .Include(t => t.TicketType)
                .SingleOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                throw new KeyNotFoundException($"Ticket #{ticketId} was not found");

            if (ticket.Order.AmountOwingCents > 0)
                throw new InvalidOperationException($"Order for ticket #{ticketId} is owing {((decimal)ticket.Order.AmountOwingCents)/100:G}");

            if (ticket.CheckInTime.HasValue)
                throw new KeyNotFoundException($"Ticket #{ticketId} was already checked in at {ticket.CheckInTime.Value}.");

            ticket.CheckInTime = DateTimeOffset.Now;

            await _db.SaveChangesAsync();
        }

        public async Task UndoCheckInTicketAsync(int ticketId, CancellationToken cancellationToken)
        {
            var ticket = await _db.Tickets
                .Include(t => t.Order)
                .Include(t => t.TicketType)
                .SingleOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                throw new KeyNotFoundException($"Ticket #{ticketId} was not found.");

            if (!ticket.CheckInTime.HasValue)
                throw new KeyNotFoundException($"Ticket #{ticketId} was not checked in.");

            ticket.CheckInTime = null;

            await _db.SaveChangesAsync();
        }
    }
}
