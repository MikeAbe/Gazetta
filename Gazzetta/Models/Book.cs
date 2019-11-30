using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Web;
using Gazzetta.FluentValidation;
using Microsoft.Owin.Security.OAuth.Messages;

namespace Gazzetta.Models
{
    public class Book 
    {
        

        public int Id { get; set; }
        [Required]
        public Publication Publication { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        public string Blurb { get; set; }
        // This property holds user-selected industry
        [Display(Name = "Category")]
        [Range(1,uint.MaxValue,ErrorMessage = "Incorrect Category")]
        [NotMapped]
        public BookCategory BookCategory { get; set; }
        // This stored human-readable name of the industry
        public virtual ApplicationUser Owner { get; set; }

        public virtual ICollection<UserBook> UserBooks { get; set; }

    }
}