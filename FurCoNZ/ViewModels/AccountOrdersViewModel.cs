﻿using FurCoNZ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FurCoNZ.ViewModels
{
    public class AccountOrdersViewModel : AccountViewModel
    {
        public List<Order> Orders { get; set; }
    }
}
