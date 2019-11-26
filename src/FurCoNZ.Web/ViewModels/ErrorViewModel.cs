using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurCoNZ.Web.ViewModels
{
    public class ErrorViewModel
    {
        //public string
        public string Title { get; set; } = "We're Sorry!";

        public string Error { get; set; } = "An unexpected error happened while processing your request.";

        public string Description { get; set; } = "If you think you encountered this page by mistake, please try again or contact us.";

        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
