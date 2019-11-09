using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FurCoNZ.Web.ViewModels;

namespace FurCoNZ.Web.Areas.Admin.ViewModels
{
    public class NewOrderViewModel : OrderIndexViewModel
    {
        public int UserId { get; set; }
    }
}
