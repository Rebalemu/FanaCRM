using System.ComponentModel.DataAnnotations;

namespace FanaCRM.ViewModels
{


    public class CompanyCreateVM
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Company Name")]
        public string Name { get; set; }

        [StringLength(100)]
        public string? Industry { get; set; }

        [Url]
        public string? Website { get; set; }

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; }

        [Required]
        [StringLength(200)]
        public string Address { get; set; }
    }
}