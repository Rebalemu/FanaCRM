using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FanaCRM.Models
{
    public class Opportunity
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        public Company Company { get; set; }

        [Display(Name = "Contact")]
        public int? ContactId { get; set; }

        public Contact? Contact { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Stage")]
        public int StageId { get; set; }

        public OpportunityStage Stage { get; set; }

        // ⚠️ Keep ONLY if you really need stored value
        [Range(0, 999999999)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime? CloseDate { get; set; }

        [Required]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [ForeignKey(nameof(AssignedTo))]
        public Users User { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<OpportunityProduct> Products { get; set; } = new List<OpportunityProduct>();
    }
}