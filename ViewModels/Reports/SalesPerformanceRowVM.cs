namespace FanaCRM.ViewModels.Reports
{
    public class SalesPerformanceRowVM
    {
        public string? SalesRepName { get; set; }

        public int TotalLeads { get; set; }

        public int ConvertedLeads { get; set; }

        public int Opportunities { get; set; }

        public int WonDeals { get; set; }

        public decimal Revenue { get; set; }

        public decimal ConversionRate { get; set; }

        public decimal WinRate { get; set; }
    }
}