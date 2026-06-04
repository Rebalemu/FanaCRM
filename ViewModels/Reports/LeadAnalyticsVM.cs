namespace FanaCRM.ViewModels.Reports
{
    public class LeadAnalyticsVM
    {
        // Filters
        public string Period { get; set; } = "Monthly";

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        // KPI Cards
        public int TotalLeads { get; set; }

        public int NewLeads { get; set; }

        public int ContactedLeads { get; set; }

        public int QualifiedLeads { get; set; }

        public int ConvertedLeads { get; set; }

        public int LostLeads { get; set; }

        public decimal ConversionRate { get; set; }

        // Insights
        public string BestLeadSource { get; set; } = "";

        public int LeadsWithoutActivity { get; set; }

        // Charts
        public List<ChartItemVM> LeadsBySource { get; set; } = new();

        public List<ChartItemVM> LeadsByStatus { get; set; } = new();

        public List<LeadAgingVM> AgingBuckets { get; set; } = new();
    }
}