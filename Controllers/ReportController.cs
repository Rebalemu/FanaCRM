using FanaCRM.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FanaCRM.Controllers
{
    [Authorize(Roles = "Admin,Sales")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;

        public ReportController(
            IReportService reportService)
        {
            _reportService = reportService;
        }

        // ======================================
        // EXECUTIVE DASHBOARD
        // ======================================

        public async Task<IActionResult> ExecutiveDashboard()
        {
            var vm =
                await _reportService
                    .GetExecutiveDashboardAsync();

            return View(vm);
        }

        // ======================================
        // LEAD REPORT
        // ======================================

        public async Task<IActionResult> LeadReport(
    string period = "Monthly",
    DateTime? startDate = null,
    DateTime? endDate = null)
        {
            var vm =
                await _reportService.GetLeadAnalyticsAsync(
                    period,
                    startDate,
                    endDate);

            return View(vm);
        }
        // ======================================
        // PIPELINE REPORT
        // ======================================

        public async Task<IActionResult> PipelineReport(
    string period = "Monthly",
    DateTime? startDate = null,
    DateTime? endDate = null)
        {
            var vm =
                await _reportService
                .GetOpportunityAnalyticsAsync(
                    period,
                    startDate,
                    endDate);

            return View(vm);
        }
        // ======================================
        // SALES PERFORMANCE
        // ======================================

        public async Task<IActionResult>
            SalesPerformance()
        {
            var vm =
                await _reportService
                    .GetSalesPerformanceAsync();

            return View(vm);
        }

        // ======================================
        // ACTIVITY REPORT
        // ======================================

        public async Task<IActionResult>
            ActivityReport()
        {
            var vm =
                await _reportService
                    .GetActivityReportAsync();

            return View(vm);
        }

    }
}