namespace FanaCRM.ViewModels
{
    public class SalesDashboardVM
    {
        public string Filter { get; set; }

        public int TotalLeads { get; set; }
        public int ClosedDeals { get; set; }
        public decimal TotalPipelineValue { get; set; }
        public decimal MonthlyRevenue { get; set; }

        public List<string> Labels { get; set; }
        public List<decimal> SalesData { get; set; }
        public List<decimal> ConversionData { get; set; }

        public int WonDeals { get; set; }

        public int LostDeals { get; set; }

    }
}