using System.ComponentModel.DataAnnotations;

namespace Gazzetta.Models
{
    public enum MagazineCategory
    {
        [Display(Name="Business")]
        Business = 1,
        [Display(Name="Entertainment")]
        Entertainment,
        [Display(Name="General")]
        General,
        [Display(Name="News")]
        News,
        [Display(Name="Other")]
        Other,
        [Display(Name="Politics")]
        Politics,
        [Display(Name="Sports")]
        Sports

    }
}