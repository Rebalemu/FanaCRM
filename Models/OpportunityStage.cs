using System.ComponentModel.DataAnnotations;

namespace FanaCRM.Models
{
    public class OpportunityStage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        [Display(Name = "Stage Name")]
        public string Name { get; set; }

        [Required]
        [Range(0, 100)]
        [Display(Name = "Probability (%)")]
        public int Probability { get; set; }
        public bool IsClosed { get; set; }

        public bool IsWon { get; set; }

        public int Order { get; set; }
    }
}
