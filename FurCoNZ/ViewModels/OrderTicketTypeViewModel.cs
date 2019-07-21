﻿// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FurCoNZ.ViewModels
{
    public class OrderTicketTypeViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int PriceCents { get; set; }
        public int TotalAvailable { get; set; }
        public int? QuantityOrdered { get; set; }
    }
}
