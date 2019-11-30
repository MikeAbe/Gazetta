using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gazzetta.Models
{
    public class UserMagazine
    {
        public int UserMagazineId { get; set; }
        public String ApplicationUserId { get; set; }
        public  ApplicationUser ApplicationUser { get; set; }
        public Guid MagazineId { get; set; }
        public  Magazine Magazine { get; set; }
        public string Status { get; set; }
        public DateTime PurchaseDate { get; set; }
        public byte Hits { get; set; }

    }
}