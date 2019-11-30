using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Gazzetta.Models
{
    public class UserEdit
    {
        public string Name { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
//        [Remote("IsEmailUnique","PhoneValidator", HttpMethod = "POST", ErrorMessage = "This Email is already registered!")]

        public string Email { get; set; }

//      [Remote("IsPhoneNumberUnique","PhoneValidator", HttpMethod = "POST", ErrorMessage = "This Phone Number is already registered!")]
        [RegularExpression("^\\+2519\\d{8}$|^\\d{7}$|^\\d{12}$")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
    }
}