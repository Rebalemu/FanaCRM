
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FanaCRM.Models
{
    public class Lead
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        [Required]
        public string Company { get; set; }

        // 🔹 Foreign Key - LeadSource
        [Display(Name = "Source")]
        [Required]
        public int SourceId { get; set; }
        public LeadSource Source { get; set; }

        // 🔹 Foreign Key - LeadStatus
        [Required]
        [Display(Name = "Status")]
        public int StatusId { get; set; }
        public LeadStatus Status { get; set; }

        // 🔹 Assigned User (ASP.NET Identity)
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [ForeignKey("AssignedTo")]
        public Users User { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
