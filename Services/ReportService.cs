using FanaCRM.Data;
using FanaCRM.Services.Interfaces;
using FanaCRM.ViewModels.Reports;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Services
{
    public class ReportService : IReportService
    {
        private readonly AppDbContext _context;

        public ReportService(AppDbContext context)
        {
            _context = context;
        }

        // ==================================================
        // EXECUTIVE DASHBOARD
        // ==================================================

        public async Task<ExecutiveDashboardVM> GetExecutiveDashboardAsync(
string period = "Monthly",
DateTime? startDate = null,
DateTime? endDate = null)
        {
            DateTime fromDate;
            DateTime toDate = DateTime.UtcNow;


            switch (period)
            {
                case "Yearly":
                    fromDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                    break;

                case "Custom":
                    fromDate = startDate ?? DateTime.UtcNow.AddMonths(-1);
                    toDate = endDate ?? DateTime.UtcNow;
                    break;

                default:
                    fromDate = new DateTime(
                        DateTime.UtcNow.Year,
                        DateTime.UtcNow.Month,
                        1);
                    break;
            }

            var leads = _context.Leads
                .Where(x => x.CreatedDate >= fromDate &&
                            x.CreatedDate <= toDate);

            var opportunities = _context.Opportunities
                .Where(x => x.CreatedDate >= fromDate &&
                            x.CreatedDate <= toDate);

            var activities = _context.Activities
                .Where(x => x.CreatedAt >= fromDate &&
                            x.CreatedAt <= toDate);

            var vm = new ExecutiveDashboardVM();

            vm.TotalLeads = await leads.CountAsync();

            vm.QualifiedLeads =
                await leads.CountAsync(x =>
                    x.Status.Name == "Qualified");

            vm.ConvertedLeads =
                await leads.CountAsync(x =>
                    x.IsConverted);

            vm.LeadConversionRate =
                vm.TotalLeads == 0
                    ? 0
                    : (decimal)vm.ConvertedLeads /
                      vm.TotalLeads * 100;

            vm.TotalOpportunities =
                await opportunities.CountAsync();

            vm.OpenOpportunities =
                await opportunities.CountAsync(x =>
                    !x.Stage.IsClosed);

            vm.WonOpportunities =
                await opportunities.CountAsync(x =>
                    x.Stage.IsClosed &&
                    x.Stage.IsWon);

            vm.LostOpportunities =
                await opportunities.CountAsync(x =>
                    x.Stage.IsClosed &&
                    !x.Stage.IsWon);

            vm.WinRate =
                (vm.WonOpportunities + vm.LostOpportunities) == 0
                    ? 0
                    : (decimal)vm.WonOpportunities /
                      (vm.WonOpportunities +
                       vm.LostOpportunities) * 100;

            vm.PipelineValue =
                await opportunities.SumAsync(x =>
                    (decimal?)x.Amount) ?? 0;

            vm.WonRevenue =
                await opportunities
                    .Where(x =>
                        x.Stage.IsClosed &&
                        x.Stage.IsWon)
                    .SumAsync(x =>
                        (decimal?)x.Amount) ?? 0;

            vm.LostRevenue =
                await opportunities
                    .Where(x =>
                        x.Stage.IsClosed &&
                        !x.Stage.IsWon)
                    .SumAsync(x =>
                        (decimal?)x.Amount) ?? 0;

            vm.TotalActivities =
                await activities.CountAsync();

            vm.CompletedActivities =
                await activities.CountAsync(x =>
                    x.IsCompleted);

            vm.PendingActivities =
                await activities.CountAsync(x =>
                    !x.IsCompleted);

            vm.LeadSources = await leads
                .GroupBy(x => x.Source.Name)
                .Select(g => new ChartItemVM
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            vm.LeadStatuses = await leads
                .GroupBy(x => x.Status.Name)
                .Select(g => new ChartItemVM
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            vm.PipelineStages = await opportunities
                .GroupBy(x => x.Stage.Name)
                .Select(g => new ChartItemVM
                {
                    Label = g.Key,
                    Count = g.Count(),
                    Amount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            vm.TopSalesPeople = await _context.Users
                .Select(u => new SalesPerformanceRowVM
                {
                    SalesRepName = u.FullName,

                    Revenue =
                        _context.Opportunities
                        .Where(o =>
                            o.AssignedTo == u.Id &&
                            o.Stage.IsClosed &&
                            o.Stage.IsWon)
                        .Sum(o => (decimal?)o.Amount) ?? 0
                })
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToListAsync();

            var topRep = vm.TopSalesPeople.FirstOrDefault();

            if (topRep != null)
            {
                vm.TopSalesRep = topRep.SalesRepName;
                vm.TopSalesRevenue = topRep.Revenue;
            }

            vm.RevenueTrend = await opportunities
                .GroupBy(x => x.CreatedDate.Month)
                .Select(g => new ChartItemVM
                {
                    Label = g.Key.ToString(),
                    Amount = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.Label)
                .ToListAsync();

            return vm;

        }


        // ==================================================
        // LEAD ANALYTICS
        // ==================================================

        public async Task<LeadAnalyticsVM> GetLeadAnalyticsAsync(
    string period = "Monthly",
    DateTime? startDate = null,
    DateTime? endDate = null)
        {
            DateTime fromDate;
            DateTime toDate = DateTime.UtcNow;

            switch (period)
            {
                case "Yearly":
                    fromDate =
                        new DateTime(
                            DateTime.UtcNow.Year,
                            1,
                            1);
                    break;

                case "Custom":
                    fromDate = startDate ?? DateTime.UtcNow.AddMonths(-1);
                    toDate = endDate ?? DateTime.UtcNow;
                    break;

                default:
                    fromDate =
                        new DateTime(
                            DateTime.UtcNow.Year,
                            DateTime.UtcNow.Month,
                            1);
                    break;
            }

            var leads = _context.Leads
                .Where(x =>
                    x.CreatedDate >= fromDate &&
                    x.CreatedDate <= toDate);

            var vm = new LeadAnalyticsVM();

            vm.TotalLeads =
                await leads.CountAsync();

            vm.NewLeads =
                await leads.CountAsync(x =>
                    x.Status.Name == "New");

            vm.ContactedLeads =
                await leads.CountAsync(x =>
                    x.Status.Name == "Contacted");

            vm.QualifiedLeads =
                await leads.CountAsync(x =>
                    x.Status.Name == "Qualified");

            vm.ConvertedLeads =
                await leads.CountAsync(x =>
                    x.IsConverted);

            vm.LostLeads =
                await leads.CountAsync(x =>
                    x.Status.Name == "Lost");

            vm.ConversionRate =
                vm.TotalLeads == 0
                    ? 0
                    : (decimal)vm.ConvertedLeads /
                      vm.TotalLeads * 100;

            vm.LeadsBySource =
                await leads
                .GroupBy(x => x.Source.Name)
                .Select(g => new ChartItemVM
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            vm.LeadsByStatus =
                await leads
                .GroupBy(x => x.Status.Name)
                .Select(g => new ChartItemVM
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            vm.LeadsWithoutActivity =
                await leads.CountAsync(x =>
                    x.LastActivityDate == null);

            vm.BestLeadSource =
                vm.LeadsBySource
                .OrderByDescending(x => x.Count)
                .Select(x => x.Label)
                .FirstOrDefault() ?? "-";

            var today = DateTime.UtcNow;

            vm.AgingBuckets = new()
    {
        new LeadAgingVM
        {
            Range = "0-7 Days",
            Count = await leads.CountAsync(x =>
                EF.Functions.DateDiffDay(
                    x.CreatedDate,
                    today) <= 7)
        },

        new LeadAgingVM
        {
            Range = "8-15 Days",
            Count = await leads.CountAsync(x =>
                EF.Functions.DateDiffDay(
                    x.CreatedDate,
                    today) >= 8 &&
                EF.Functions.DateDiffDay(
                    x.CreatedDate,
                    today) <= 15)
        },

        new LeadAgingVM
        {
            Range = "16-30 Days",
            Count = await leads.CountAsync(x =>
                EF.Functions.DateDiffDay(
                    x.CreatedDate,
                    today) >= 16 &&
                EF.Functions.DateDiffDay(
                    x.CreatedDate,
                    today) <= 30)
        },

        new LeadAgingVM
        {
            Range = "30+ Days",
            Count = await leads.CountAsync(x =>
                EF.Functions.DateDiffDay(
                    x.CreatedDate,
                    today) > 30)
        }
    };

            return vm;
        }

        // ==================================================
        // PIPELINE REPORT
        // ==================================================

        public async Task<PipelineReportVM> GetPipelineReportAsync()
        {
            var stages = await _context.Opportunities
                .GroupBy(x => new
                {
                    x.Stage.Name,
                    x.Stage.Probability
                })
                .Select(g => new PipelineStageVM
                {
                    StageName = g.Key.Name,
                    Probability = g.Key.Probability,
                    OpportunityCount = g.Count(),
                    TotalValue = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            var won =
                await _context.Opportunities
                    .CountAsync(x => x.Stage.IsClosed && x.Stage.IsWon);

            var lost =
                await _context.Opportunities
                    .CountAsync(x => x.Stage.IsClosed && !x.Stage.IsWon);

            return new PipelineReportVM
            {
                TotalOpportunities =
                    await _context.Opportunities.CountAsync(),

                OpenOpportunities =
                    await _context.Opportunities
                        .CountAsync(x => !x.Stage.IsClosed),

                WonOpportunities = won,

                LostOpportunities = lost,

                TotalPipelineValue =
                    await _context.Opportunities
                        .SumAsync(x => x.Amount),

                WonRevenue =
                    await _context.Opportunities
                        .Where(x => x.Stage.IsClosed && x.Stage.IsWon)
                        .SumAsync(x => x.Amount),

                LostRevenue =
                    await _context.Opportunities
                        .Where(x => x.Stage.IsClosed && !x.Stage.IsWon)
                        .SumAsync(x => x.Amount),

                WinRate =
                    (won + lost) == 0
                    ? 0
                    : (decimal)won / (won + lost) * 100,

                StageBreakdown = stages
            };
        }
        
        // ==================================================
        // OPPORTUNTY ANALYTICS
        // ==================================================
        
        public async Task<OpportunityAnalyticsVM> GetOpportunityAnalyticsAsync(
    string period = "Monthly",
    DateTime? startDate = null,
    DateTime? endDate = null)
        {
            DateTime fromDate;
            DateTime toDate = DateTime.UtcNow;

            switch (period)
            {
                case "Yearly":
                    fromDate = new DateTime(DateTime.UtcNow.Year, 1, 1);
                    break;

                case "Custom":
                    fromDate = startDate ?? DateTime.UtcNow.AddMonths(-1);
                    toDate = endDate ?? DateTime.UtcNow;
                    break;

                default:
                    fromDate = new DateTime(
                        DateTime.UtcNow.Year,
                        DateTime.UtcNow.Month,
                        1);
                    break;
            }

            var opportunities = _context.Opportunities
                .Where(x =>
                    x.CreatedDate >= fromDate &&
                    x.CreatedDate <= toDate);

            var vm = new OpportunityAnalyticsVM();

            vm.TotalOpportunities =
                await opportunities.CountAsync();

            vm.OpenOpportunities =
                await opportunities.CountAsync(x =>
                    !x.Stage.IsClosed);

            vm.WonOpportunities =
                await opportunities.CountAsync(x =>
                    x.Stage.IsClosed &&
                    x.Stage.IsWon);

            vm.LostOpportunities =
                await opportunities.CountAsync(x =>
                    x.Stage.IsClosed &&
                    !x.Stage.IsWon);

            vm.PipelineValue =
                await opportunities.SumAsync(x =>
                    (decimal?)x.Amount) ?? 0;

            vm.WonRevenue =
                await opportunities
                .Where(x =>
                    x.Stage.IsWon)
                .SumAsync(x =>
                    (decimal?)x.Amount) ?? 0;

            vm.LostRevenue =
                await opportunities
                .Where(x =>
                    x.Stage.IsClosed &&
                    !x.Stage.IsWon)
                .SumAsync(x =>
                    (decimal?)x.Amount) ?? 0;

            vm.WinRate =
                (vm.WonOpportunities + vm.LostOpportunities) == 0
                ? 0
                : (decimal)vm.WonOpportunities /
                  (vm.WonOpportunities +
                   vm.LostOpportunities) * 100;

            vm.AverageDealSize =
                vm.TotalOpportunities == 0
                ? 0
                : vm.PipelineValue /
                  vm.TotalOpportunities;

            vm.OpportunitiesByStage =
                await opportunities
                .GroupBy(x => x.Stage.Name)
                .Select(g => new ChartItemVM
                {
                    Label = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            vm.RevenueByStage =
                await opportunities
                .GroupBy(x => x.Stage.Name)
                .Select(g => new ChartItemVM
                {
                    Label = g.Key,
                    Amount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            vm.RevenueTrend =
                await opportunities
                .GroupBy(x => x.CreatedDate.Month)
                .Select(g => new ChartItemVM
                {
                    Label = g.Key.ToString(),
                    Amount = g.Sum(x => x.Amount)
                })
                .OrderBy(x => x.Label)
                .ToListAsync();

            vm.TopSalesPeople =
                await _context.Users
                .Select(u => new SalesPerformanceRowVM
                {
                    SalesRepName = u.FullName,

                    Revenue =
                        _context.Opportunities
                        .Where(o =>
                            o.AssignedTo == u.Id &&
                            o.Stage.IsWon)
                        .Sum(o =>
                            (decimal?)o.Amount) ?? 0
                })
                .OrderByDescending(x => x.Revenue)
                .Take(5)
                .ToListAsync();

            var topRep =
                vm.TopSalesPeople.FirstOrDefault();

            if (topRep != null)
            {
                vm.TopSalesRep =
                    topRep.SalesRepName;
            }

            return vm;
        }

        // ==================================================
        // SALES PERFORMANCE
        // ==================================================

        public async Task<SalesPerformanceVM> GetSalesPerformanceAsync()
        {
            var rows = await _context.Users
                .Select(user => new SalesPerformanceRowVM
                {
                    SalesRepName = user.FullName,

                    TotalLeads =
                        _context.Leads.Count(l =>
                            l.AssignedTo == user.Id),

                    ConvertedLeads =
                        _context.Leads.Count(l =>
                            l.AssignedTo == user.Id &&
                            l.IsConverted),

                    Opportunities =
                        _context.Opportunities.Count(o =>
                            o.AssignedTo == user.Id),

                    WonDeals =
                        _context.Opportunities.Count(o =>
                            o.AssignedTo == user.Id &&
                            o.Stage.IsClosed &&
                            o.Stage.IsWon),

                    Revenue =
                        _context.Opportunities
                            .Where(o =>
                                o.AssignedTo == user.Id &&
                                o.Stage.IsClosed &&
                                o.Stage.IsWon)
                            .Sum(o => (decimal?)o.Amount) ?? 0
                })
                .ToListAsync();

            foreach (var row in rows)
            {
                row.ConversionRate =
                    row.TotalLeads == 0
                    ? 0
                    : (decimal)row.ConvertedLeads /
                      row.TotalLeads * 100;

                row.WinRate =
                    row.Opportunities == 0
                    ? 0
                    : (decimal)row.WonDeals /
                      row.Opportunities * 100;
            }

            return new SalesPerformanceVM
            {
                SalesPeople = rows
                    .OrderByDescending(x => x.Revenue)
                    .ToList()
            };
        }

        // ==================================================
        // ACTIVITY REPORT
        // ==================================================

        public async Task<ActivityReportVM> GetActivityReportAsync()
        {
            var vm = new ActivityReportVM();

            vm.TotalActivities =
                await _context.Activities.CountAsync();

            vm.CompletedActivities =
                await _context.Activities
                    .CountAsync(x => x.IsCompleted);

            vm.PendingActivities =
                await _context.Activities
                    .CountAsync(x => !x.IsCompleted);

            vm.OverdueActivities =
                await _context.Activities
                    .CountAsync(x =>
                        !x.IsCompleted &&
                        x.DueDate < DateTime.UtcNow);

            vm.ActivitiesByType =
                await _context.Activities
                    .GroupBy(x => x.ActivityType.Name)
                    .Select(g => new ChartItemVM
                    {
                        Label = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

            vm.ActivitiesByStatus =
                await _context.Activities
                    .GroupBy(x => x.ActivityStatus.Name)
                    .Select(g => new ChartItemVM
                    {
                        Label = g.Key,
                        Count = g.Count()
                    })
                    .ToListAsync();

            return vm;
        }

        // ==================================================
        // STAGE DURATION REPORT
        // ==================================================


    }
}