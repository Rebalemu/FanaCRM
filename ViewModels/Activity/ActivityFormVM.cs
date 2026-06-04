using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FanaCRM.ViewModels
{
    public class ActivityFormVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Activity Type")]
        public int TypeId { get; set; }

        [Required]
        [StringLength(200)]
        public string Subject { get; set; }

        public string? Description { get; set; }

        [Display(Name = "Company")]
        public int? CompanyId { get; set; }

        [Display(Name = "Contact")]
        public int? ContactId { get; set; }

        [Display(Name = "Lead")]
        public int? LeadId { get; set; }

        [Display(Name = "Opportunity")]
        public int? OpportunityId { get; set; }

        [Required]
        [Display(Name = "Assigned To")]
        public string AssignedToId { get; set; }

        [Display(Name = "Due Date")]
        public DateTime? DueDate { get; set; }

        [Required]
        [Display(Name = "Status")]
        public int StatusId { get; set; }

        // DROPDOWNS
        public List<SelectListItem> Types { get; set; } = new();

        public List<SelectListItem> Statuses { get; set; } = new();

        public List<SelectListItem> Companies { get; set; } = new();

        public List<SelectListItem> Contacts { get; set; } = new();

        public List<SelectListItem> Leads { get; set; } = new();

        public List<SelectListItem> Opportunities { get; set; } = new();

        public List<SelectListItem> Users { get; set; } = new();
    }
}