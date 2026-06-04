using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FanaCRM.Models
{
    public class LeadCreateVM
    {
        public int? Id { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public string Company { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Source")]
        public int SourceId { get; set; }

        public List<SelectListItem> Sources { get; set; } = new();

        [Required]
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        public List<SelectListItem> Statuses { get; set; } = new();

        [Display(Name = "Assigned To")]
        public string? AssignedTo { get; set; }

        public List<SelectListItem> Users { get; set; } = new();
    }
}