using FanaCRM.ViewModels.Reports;

namespace FanaCRM.Services.Interfaces
{
    public interface IReportService
    {
        Task<ExecutiveDashboardVM> GetExecutiveDashboardAsync(
 string period = "Monthly",
 DateTime? startDate = null,
 DateTime? endDate = null);


        Task<LeadAnalyticsVM> GetLeadAnalyticsAsync(
    string period = "Monthly",
    DateTime? startDate = null,
    DateTime? endDate = null);
        Task<OpportunityAnalyticsVM> GetOpportunityAnalyticsAsync(
        string period = "Monthly",
        DateTime? startDate = null,
        DateTime? endDate = null);

        Task<PipelineReportVM> GetPipelineReportAsync();

        Task<SalesPerformanceVM> GetSalesPerformanceAsync();

        Task<ActivityReportVM> GetActivityReportAsync();
    }
}