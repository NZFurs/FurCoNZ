#if DEBUG
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FurCoNZ.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FurCoNZ.Web.Areas.Debug.Controllers
{
    [Area("Debug")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Exception()
        {
            throw new NotImplementedException();
        }

        public IActionResult Error()
        {
            return View("~/Views/Shared/Error.cshtml", new ErrorViewModel
            {
                Title = "This is a test",
                Error = "If you're seeing this, Don't Panic! Always remember your towel.",
            });
        }
    }
}
#endif
