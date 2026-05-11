using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FanaCRM.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Activity Type")]
        public int TypeId { get; set; }

        public ActivityType Type { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; }

        public string Description { get; set; }

        // Related Account (Company)
        [Display(Name = "Company")]
        public int? CompanyId { get; set; }

        public Company Company { get; set; }

        // Related Contact
        public int? ContactId { get; set; }

        public Contact Contact { get; set; }

        // Assigned User
        [Required]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [ForeignKey(nameof(AssignedTo))]
        public Users User { get; set; }

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } // Open, Completed, Cancelled

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // 🔹 Related Lead
        public int? LeadId { get; set; }

        public Lead Lead { get; set; }
    }
}