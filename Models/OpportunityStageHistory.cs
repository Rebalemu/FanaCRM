using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FanaCRM.Models
{
    public class OpportunityStageHistory
    {
        public int Id { get; set; }

        [Required]
        public int OpportunityId { get; set; }

        [Required]
        public int StageId { get; set; }

        [Required]
        public DateTime EnteredAt { get; set; }

        public DateTime? ExitedAt { get; set; }

        public string ChangedByUserId { get; set; }

        // 🔗 Navigation Properties

        [ForeignKey("OpportunityId")]
        public Opportunity Opportunity { get; set; }

        [ForeignKey("StageId")]
        public OpportunityStage Stage { get; set; }

        [ForeignKey("ChangedByUserId")]
        public Users ChangedByUser { get; set; }
    }
}