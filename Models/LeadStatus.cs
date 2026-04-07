
using System.ComponentModel.DataAnnotations;
namespace FanaCRM.Models
{
    public class LeadStatus
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        // 🔹 Navigation property
        public ICollection<Lead> Leads { get; set; }
    }

}
