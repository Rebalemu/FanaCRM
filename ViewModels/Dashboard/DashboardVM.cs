namespace FanaCRM.ViewModels
{
    public class DashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalContacts { get; set; }
        public int TotalCompanies { get; set; }

        public int TotalLeads { get; set; }
        public int ConvertedLeads { get; set; }

        public double ConversionRate { get; set; }

        public List<string> Labels { get; set; } = new();
        public List<int> LeadsData { get; set; } = new();
        public List<decimal> RevenueData { get; set; } = new();

        public string Filter { get; set; } = "month";
    }
}