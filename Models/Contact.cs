using System.ComponentModel.DataAnnotations;

namespace FanaCRM.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }
        public Company? Company { get; set; }
        [Required]
        [StringLength(50)]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string Phone { get; set; }
        public string? Position { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<Opportunity> Opportunities { get; set; }
        public ICollection<Activity> Activities { get; set; }
    }
}