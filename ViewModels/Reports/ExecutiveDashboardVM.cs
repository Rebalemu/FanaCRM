namespace FanaCRM.ViewModels.Reports
{
public class ExecutiveDashboardVM
{
// Filters
public string Period { get; set; } = "Monthly";


    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    // Lead KPIs
    public int TotalLeads { get; set; }

    public int QualifiedLeads { get; set; }

    public int ConvertedLeads { get; set; }

    public decimal LeadConversionRate { get; set; }

    // Opportunity KPIs
    public int TotalOpportunities { get; set; }

    public int OpenOpportunities { get; set; }

    public int WonOpportunities { get; set; }

    public int LostOpportunities { get; set; }

    public decimal WinRate { get; set; }

    // Revenue
    public decimal PipelineValue { get; set; }

    public decimal WonRevenue { get; set; }

    public decimal LostRevenue { get; set; }

    // Activity KPIs
    public int TotalActivities { get; set; }

    public int CompletedActivities { get; set; }

    public int PendingActivities { get; set; }

    // Top Performer
    public string TopSalesRep { get; set; } = "";

    public decimal TopSalesRevenue { get; set; }

    // Charts
    public List<ChartItemVM> LeadSources { get; set; } = new();

    public List<ChartItemVM> LeadStatuses { get; set; } = new();

    public List<ChartItemVM> PipelineStages { get; set; } = new();

    public List<ChartItemVM> RevenueTrend { get; set; } = new();

    public List<SalesPerformanceRowVM> TopSalesPeople { get; set; } = new();
}


}
