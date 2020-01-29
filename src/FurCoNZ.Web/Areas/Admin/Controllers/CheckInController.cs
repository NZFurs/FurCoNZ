using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FurCoNZ.Web.Services;
using FurCoNZ.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FurCoNZ.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class CheckInController : Controller
    {
        private readonly IOrderService _orderService;

        public CheckInController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
        {
            var tickets = await _orderService.GetDetailedAttendeeListAsync(includeExpiredOrders: false, cancellationToken);

            return View(tickets.Select(t => new TicketDetailViewModel(t)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckInTicket(int ticketId, CancellationToken cancellationToken = default)
        {
            await _orderService.CheckInTicketAsync(ticketId, cancellationToken); // Sets Ticket.CheckInTime (new column) to DateTimeOffset.Now()

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UndoCheckInTicket(int ticketId, CancellationToken cancellationToken = default)
        {
            await _orderService.UndoCheckInTicketAsync(ticketId, cancellationToken); // Sets Ticket.CheckInTime (new column) to null

            return RedirectToAction(nameof(Index));
        }
    }
}
