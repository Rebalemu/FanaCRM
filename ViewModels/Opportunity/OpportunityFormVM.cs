using System.ComponentModel.DataAnnotations;
using FanaCRM.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FanaCRM.ViewModels
{

    public class OpportunityFormVM
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }
        [Display(Name = "Contact")]

        public int? ContactId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Stage")]
        public int StageId { get; set; }

        public DateTime? CloseDate { get; set; }

        [Required]
        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        public string? LossReason { get; set; }

        public List<OpportunityProductVM> Products { get; set; } = new();
        public List<Product> ProductsData { get; set; } = new List<Product>();

        // Dropdowns
        public List<SelectListItem> Companies { get; set; } = new();
        public List<SelectListItem> Contacts { get; set; } = new();
        public List<SelectListItem> Stages { get; set; } = new();
        public List<SelectListItem> Users { get; set; } = new();
        public List<SelectListItem> ProductList { get; set; } = new();
        public List<int> LostStageIds { get; set; } = new();


        // Calculated
        public decimal TotalAmount => Products.Sum(p => p.Total);
    }
}