using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FanaCRM.Models
{
    public class Lead
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public string Company { get; set; }

        // 🔹 Source
        [Required]
        [Display(Name = "Source")]
        public int SourceId { get; set; }
        public LeadSource Source { get; set; }

        // 🔹 Status
        [Required]
        [Display(Name = "Status")]
        public int StatusId { get; set; }
        public LeadStatus Status { get; set; }

        // 🔹 Assigned User
        [Display(Name = "Assigned To")]
        public string? AssignedTo { get; set; }

        [ForeignKey(nameof(AssignedTo))]
        public Users User { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}