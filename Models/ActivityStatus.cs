using System.ComponentModel.DataAnnotations;

namespace FanaCRM.Models
{
    public class ActivityStatus
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } //penf

        public ICollection<Activity> Activities { get; set; }
    }

    
}