using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;

using FurCoNZ.Web.ViewModels;

namespace FurCoNZ.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetNotified()
        {
            return View();
        }

        public IActionResult News()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Tickets()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }
        public IActionResult Terms()
        {
            return View();
        }

        public IActionResult ParentalConsent()
        {
            return View();
        }

        public IActionResult AboutVenue()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Title = "We're Sorry!",
            };

            if (exceptionHandlerPathFeature.Error != null)
                errorViewModel.Error = exceptionHandlerPathFeature.Error.Message;

            return View("~/Views/Shared/Error.cshtml", errorViewModel);
        }

        [AllowAnonymous]
        public IActionResult StatusCode(int code)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                Title = $"{code} - {ReasonPhrases.GetReasonPhrase(code)}",
                Error = string.Empty,
            };

            return View("~/Views/Shared/Error.cshtml", errorViewModel);
        }
    }
}
