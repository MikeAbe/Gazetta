using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Gazzetta.Models
{
    public enum BookCategory
    {
        [Display(Name="Autobiography")]
        Autobiography = 1,
        [Display(Name="Business")]
        Business,
        [Display(Name="Culinary")]
        Culinary,
        [Display(Name="Education")]
        Education,
        [Display(Name="Fiction")]
        Fiction,
        [Display(Name="Other")]
        Other,
        [Display(Name="Politics")]
        Poltics,
        [Display(Name="Psychology")]
        Psychology,
        [Display(Name = "Religion")]
        Religion,
        [Display(Name="Self Help")]
        SelfHelp
    }
}