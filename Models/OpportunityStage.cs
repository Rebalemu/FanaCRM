using System.ComponentModel.DataAnnotations;

namespace FanaCRM.Models
{
    public class OpportunityStage
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [Range(0, 100)]
        public int Probability { get; set; }
    }
}