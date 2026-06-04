using System.ComponentModel.DataAnnotations;

namespace FanaCRM.ViewModels
{
    public class OpportunityDetailsVM
    {
        // =========================
        // BASIC
        // =========================
        public int Id { get; set; }

        [Display(Name = "Opportunity Name")]
        public string Name { get; set; } = string.Empty;

        // =========================
        // COMPANY / CONTACT
        // =========================
        public int CompanyId { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; } = string.Empty;

        public int? ContactId { get; set; }

        [Display(Name = "Contact")]
        public string? ContactName { get; set; }

        // =========================
        // PIPELINE
        // =========================
        public int StageId { get; set; }

        [Display(Name = "Stage")]
        public string StageName { get; set; } = string.Empty;

        [Display(Name = "Probability")]
        public int Probability { get; set; }

        // =========================
        // SALES
        // =========================
        [Display(Name = "Amount")]
        [DisplayFormat(DataFormatString = "{0:N2}")]
        public decimal Amount { get; set; }

        [Display(Name = "Expected Close Date")]
        public DateTime? CloseDate { get; set; }

        // =========================
        // ASSIGNMENT
        // =========================
        public string AssignedToId { get; set; } = string.Empty;

        [Display(Name = "Assigned To")]
        public string AssignedToName { get; set; } = string.Empty;

        // =========================
        // LOST INFO
        // =========================
        [Display(Name = "Loss Reason")]
        public string? LossReason { get; set; }

        // =========================
        // PRODUCTS
        // =========================
        public List<OpportunityProductVM> Products { get; set; } = new();
        public List<ActivityWidgetVM> UpcomingActivities { get; set; }
    = new();
        public List<TimelineEventVM> Timeline { get; set; }
    = new();
    }
}