using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FanaCRM.Models
{
    public class Activity
    {
        public int Id { get; set; }

        // BASIC INFO
        [Required]
        [StringLength(200)]
        public string Subject { get; set; }

        public string? Description { get; set; }

        // TYPE
        [Required]
        public int ActivityTypeId { get; set; }
        public ActivityType ActivityType { get; set; }

        // STATUS
        [Required]
        public int ActivityStatusId { get; set; }
        public ActivityStatus ActivityStatus { get; set; }

        // RELATIONS
        public int? LeadId { get; set; }
        public Lead? Lead { get; set; }

        public int? OpportunityId { get; set; }
        public Opportunity? Opportunity { get; set; }

        public int? ContactId { get; set; }
        public Contact? Contact { get; set; }

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }

        // ASSIGNMENT
        [Required]
        public string AssignedToId { get; set; }

        [ForeignKey(nameof(AssignedToId))]
        public Users AssignedTo { get; set; }

        // TRACKING
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? DueDate { get; set; }

        public DateTime? CompletedAt { get; set; }

        public bool IsCompleted { get; set; } = false;

        // AUDIT
        public string? CreatedById { get; set; }

        public string? UpdatedById { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}