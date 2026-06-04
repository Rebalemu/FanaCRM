using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FanaCRM.ViewModels
{
    public class LeadEditVM
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }

        [Required]
        public int SourceId { get; set; }

        [Required]
        public int StatusId { get; set; }

        public string? AssignedTo { get; set; }

        public List<SelectListItem>? Sources { get; set; }
        public List<SelectListItem>? Statuses { get; set; }
        public List<SelectListItem>? Users { get; set; }
    }
}