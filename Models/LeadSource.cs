using System.ComponentModel.DataAnnotations;

namespace FanaCRM.Models
{
    public class LeadSource
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public ICollection<Lead>? Leads { get; set; }
    }
}