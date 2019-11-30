using System.Linq;
using System.Web;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Gazzetta.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        
    }
}