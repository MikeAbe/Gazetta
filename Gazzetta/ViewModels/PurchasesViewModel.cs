using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gazzetta.Models;

namespace Gazzetta.ViewModels
{
    public class PurchasesViewModel
    {
        public List<Book> Books { get; set; }
        public List<Magazine> Magazines { get; set; }
    }
}