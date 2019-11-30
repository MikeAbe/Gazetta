using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Gazzetta.Models
{
    public class Magazine        
    {
        
        public Publication Publication { get; set; }


        public Guid Id { get; set; }
        [Required]
//        [Column(TypeName = "date")]
        [DataType(DataType.Date)]
        [DisplayName("Issue Number")]
        public DateTime IssueNumber { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        [DisplayName("Category")]
        [Range(1,uint.MaxValue,ErrorMessage = "Incorrect Category")]
        [NotMapped]
        public MagazineCategory MagazineCategory { get; set; }

        public virtual ICollection<UserMagazine> UserMagazines { get; set; }

    }
}