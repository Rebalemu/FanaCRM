using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FanaCRM.Models
{
    public class ActivityType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public ICollection<Activity> Activities { get; set; }
    }
}