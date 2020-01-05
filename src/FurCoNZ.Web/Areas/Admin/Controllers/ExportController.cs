using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FurCoNZ.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FurCoNZ.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Policy = "AdminOnly")]
    public class ExportController : Controller
    {
        private readonly IOrderService _orderService;

        public ExportController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Attendence(CancellationToken cancellationToken = default)
        {
            // TODO: Use formatter: https://github.com/damienbod/WebAPIContrib.Core/tree/master/src/WebApiContrib.Core.Formatter.Csv

            var tickets = await _orderService.GetDetailedAttendeeListAsync(cancellationToken: cancellationToken);

            var csv = new StringBuilder();

            csv.AppendLine("\"Order Id\",\"Order Placed At\",\"Order Placed By\",\"Order Paid Amount\",\"Ticket Id\",\"Ticket Type\",\"Badge Name\",\"Legal Name\",\"Preferred Name\",\"Order Account Email Address\",\"Ticket Holder Email Address\",\"Ticket Holder Date of Birth\",\"Meal Requirements (flags)\",\"Medical Requirements\",\"Cabin Preferences\",\"Additional Notes\",\"Accepted T&C\"");

            foreach(var ticket in tickets)
            {
                csv.AppendJoin(',',
                    ticket.Order.Id,
                    ticket.Order.CreatedAt,
                    $"\"{ticket.Order.OrderedBy.Name}\"",
                    ticket.Order.AmountPaidCents,
                    ticket.Id,
                    ticket.TicketType.Name,
                    $"\"{ticket.TicketName}\"",
                    $"\"{ticket.LegalName}\"",
                    $"\"{ticket.PreferredName}\"",
                    ticket.Order.OrderedBy.Email,
                    ticket.EmailAddress,
                    ticket.DateOfBirth,
                    $"\"{ticket.MealRequirements}\"",
                    $"\"{ticket.KnownAllergens}\"",
                    $"\"{ticket.CabinGrouping}\"",
                    $"\"{ticket.AdditionalNotes}\"",
                    ticket.AcceptedTermsAndConditions
                );
                csv.Append(Environment.NewLine);
            }

            Response.Headers.Add("content-disposition", $"attachment; filename=attendence-{DateTime.Now.ToString("yyyy-MM-dd")}.csv");
            return Content(csv.ToString(), "text/csv");

            // Response.ContentType = "application/CSV";
            

            // return Ok(csv.ToString());
        }
    }
}
