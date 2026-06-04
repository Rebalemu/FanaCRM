namespace FanaCRM.ViewModels.Reports
{
    public class OpportunityAnalyticsVM
    {
        public string Period { get; set; } = "Monthly";

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        // KPI Cards
        public int TotalOpportunities { get; set; }

        public int OpenOpportunities { get; set; }

        public int WonOpportunities { get; set; }

        public int LostOpportunities { get; set; }

        public decimal WinRate { get; set; }

        public decimal PipelineValue { get; set; }

        public decimal WonRevenue { get; set; }

        public decimal LostRevenue { get; set; }

        // Insights
        public string TopSalesRep { get; set; } = "";

        public decimal AverageDealSize { get; set; }

        // Charts
        public List<ChartItemVM> OpportunitiesByStage { get; set; } = new();

        public List<ChartItemVM> RevenueByStage { get; set; } = new();

        public List<ChartItemVM> RevenueTrend { get; set; } = new();

        public List<SalesPerformanceRowVM> TopSalesPeople { get; set; } = new();
    }
}