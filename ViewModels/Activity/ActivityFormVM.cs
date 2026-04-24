using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FanaCRM.ViewModels
{
    public class ActivityFormVM
    {
        public int Id { get; set; } // 0 = Create

        [Required]
        public int TypeId { get; set; }

        [Required]
        public string Subject { get; set; }

        public string Description { get; set; }

        public int? CompanyId { get; set; }

        public int? ContactId { get; set; }

        [Required]
        public string AssignedTo { get; set; }

        public DateTime? DueDate { get; set; }

        [Required]
        public string Status { get; set; }

        public List<SelectListItem> Types { get; set; } = new();
        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Contacts { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();
    }
}