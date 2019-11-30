using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Gazzetta.Models;

namespace Gazzetta.ViewModels
{
    public class PurchViewModel
    {
        public List<UserMagazine> UserMagazines { get; set; }
        public List<UserBook> UserBooks { get; set; }
    }
}