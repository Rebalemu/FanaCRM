using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FanaCRM.Models
{
    public class TimelineEvent
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        public string? Description { get; set; }

        

        [Required]
        [StringLength(100)]
        public string EventType { get; set; }

        public DateTime CreatedDate { get; set; }
            = DateTime.UtcNow;

        // USER
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public Users User { get; set; }

        // RELATIONS
        public int? LeadId { get; set; }
        public Lead? Lead { get; set; }

        public int? OpportunityId { get; set; }
        public Opportunity? Opportunity { get; set; }

        public int? ActivityId { get; set; }
        public Activity? Activity { get; set; }
    }
}