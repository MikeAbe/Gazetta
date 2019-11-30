using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
using System.Web;

namespace Gazzetta.Models
{
    public class Publication
    {
       
        [Required]
        public string Name { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string Publisher { get; set; }

        public string Category { get; set; }
        [Required]
        public string Language { get; set; }

        public string Description { get; set; }
        public byte [] Content { get; set; }
        public byte[] Thumbnail { get; set; }
        public string MediaType { get; set; }
        public FileType FileType { get; set; }
        public string Tags { get; set; }
        public int Downloaded { get; set; }



        //Lazy loading customer/owner of publication using the virtual Key word






        //Lazy loading the following two lines
        // public UserBooks UserBooks { get; set; }
        // public int CustomersId { get; set; } // by convention foreign key

        //Lazy loading one to many association
        //public virtual UserBooks UserBooks { get; set; }











    }
}