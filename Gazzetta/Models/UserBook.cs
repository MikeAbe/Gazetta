using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gazzetta.Models
{
    public class UserBook
    {
        public int UserBookId { get; set; }
        public string AppliationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }
        public int BookId { get; set; }
        public virtual Book Book { get; set; }
        public string Status { get; set; }
        public DateTime PurchaseDate { get; set; }
        public byte Hits { get; set; }



    }
}