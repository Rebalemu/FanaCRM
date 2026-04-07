
using System.ComponentModel.DataAnnotations;

namespace FanaCRM.Models
{
    public class LeadSource
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        // 🔹 Navigation property (one-to-many)
        public ICollection<Lead> Leads { get; set; }
    }
}
