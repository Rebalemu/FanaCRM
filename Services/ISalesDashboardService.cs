using FanaCRM.Data;
using FanaCRM.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Services
{
    public interface ISalesDashboardService
    {
        Task<SalesDashboardVM> GetDashboardDataAsync(string filter);
    }

    public class SalesDashboardService : ISalesDashboardService
    {
        private readonly AppDbContext _context;

        public SalesDashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<SalesDashboardVM> GetDashboardDataAsync(string filter)
        {
            filter ??= "month";

            var vm = new SalesDashboardVM
            {
                Filter = filter
            };

            DateTime fromDate = GetFromDate(filter);

            // =========================
            // TOTAL LEADS
            // =========================
            vm.TotalLeads = await _context.Leads
                .CountAsync(x => x.CreatedDate >= fromDate);

            // =========================
            // WON DEALS
            // =========================
            vm.WonDeals = await _context.Opportunities
                .Include(o => o.Stage)
                .CountAsync(o =>
                    o.Stage.IsClosed &&
                    o.Stage.IsWon &&
                    o.CloseDate >= fromDate);

            // =========================
            // LOST DEALS
            // =========================
            vm.LostDeals = await _context.Opportunities
                .Include(o => o.Stage)
                .CountAsync(o =>
                    o.Stage.IsClosed &&
                    !o.Stage.IsWon &&
                    o.CloseDate >= fromDate);

            // =========================
            // TOTAL CLOSED DEALS
            // =========================
            vm.ClosedDeals = vm.WonDeals + vm.LostDeals;
            // =========================
            // TOTAL PIPELINE VALUE
            // =========================
            vm.TotalPipelineValue = await _context.Opportunities
                .Include(o => o.Stage)
                .Where(o => !o.Stage.IsClosed)
                .SumAsync(o => (decimal?)o.Amount) ?? 0;

            // =========================
            // MONTHLY REVENUE
            // =========================
            vm.MonthlyRevenue = await _context.Opportunities
                .Include(o => o.Stage)
                .Where(o =>
                    o.Stage.IsWon &&
                    o.CloseDate >= fromDate)
                .SumAsync(o => (decimal?)o.Amount) ?? 0;

            // =========================
            // LABELS
            // =========================
            vm.Labels = GetLabels(filter);

            // =========================
            // SALES TREND DATA
            // =========================
            vm.SalesData = new List<decimal>();

            // =========================
            // CONVERSION RATE DATA
            // =========================
            vm.ConversionData = new List<decimal>();

            if (filter == "today")
            {
                foreach (var hour in Enumerable.Range(0, 6).Reverse())
                {
                    var start = DateTime.Now.Date.AddHours(DateTime.Now.Hour - hour);
                    var end = start.AddHours(1);

                    // Revenue
                    var revenue = await _context.Opportunities
                        .Include(o => o.Stage)
                        .Where(o =>
                            o.Stage.IsWon &&
                            o.CloseDate >= start &&
                            o.CloseDate < end)
                        .SumAsync(o => (decimal?)o.Amount) ?? 0;

                    vm.SalesData.Add(revenue);

                    // Conversion
                    var total = await _context.Opportunities
                        .CountAsync(o =>
                            o.CreatedDate >= start &&
                            o.CreatedDate < end);

                    var won = await _context.Opportunities
                        .Include(o => o.Stage)
                        .CountAsync(o =>
                            o.Stage.IsWon &&
                            o.CreatedDate >= start &&
                            o.CreatedDate < end);

                    decimal conversion = total == 0
                        ? 0
                        : ((decimal)won / total) * 100;

                    vm.ConversionData.Add(Math.Round(conversion, 2));
                }
            }
            else if (filter == "week")
            {
                foreach (var day in Enumerable.Range(0, 7).Reverse())
                {
                    var start = DateTime.Today.AddDays(-day);
                    var end = start.AddDays(1);

                    // Revenue
                    var revenue = await _context.Opportunities
                        .Include(o => o.Stage)
                        .Where(o =>
                            o.Stage.IsWon &&
                            o.CloseDate >= start &&
                            o.CloseDate < end)
                        .SumAsync(o => (decimal?)o.Amount) ?? 0;

                    vm.SalesData.Add(revenue);

                    // Conversion
                    var total = await _context.Opportunities
                        .CountAsync(o =>
                            o.CreatedDate >= start &&
                            o.CreatedDate < end);

                    var won = await _context.Opportunities
                        .Include(o => o.Stage)
                        .CountAsync(o =>
                            o.Stage.IsWon &&
                            o.CreatedDate >= start &&
                            o.CreatedDate < end);

                    decimal conversion = total == 0
                        ? 0
                        : ((decimal)won / total) * 100;

                    vm.ConversionData.Add(Math.Round(conversion, 2));
                }
            }
            else
            {
                foreach (var period in Enumerable.Range(0, 6).Reverse())
                {
                    var start = DateTime.Today.AddDays(-(period * 5));
                    var end = start.AddDays(5);

                    // Revenue
                    var revenue = await _context.Opportunities
                        .Include(o => o.Stage)
                        .Where(o =>
                            o.Stage.IsWon &&
                            o.CloseDate >= start &&
                            o.CloseDate < end)
                        .SumAsync(o => (decimal?)o.Amount) ?? 0;

                    vm.SalesData.Add(revenue);

                    // Conversion
                    var total = await _context.Opportunities
                        .CountAsync(o =>
                            o.CreatedDate >= start &&
                            o.CreatedDate < end);

                    var won = await _context.Opportunities
                        .Include(o => o.Stage)
                        .CountAsync(o =>
                            o.Stage.IsWon &&
                            o.CreatedDate >= start &&
                            o.CreatedDate < end);

                    decimal conversion = total == 0
                        ? 0
                        : ((decimal)won / total) * 100;

                    vm.ConversionData.Add(Math.Round(conversion, 2));
                }
            }

            return vm;
        }

        // =========================
        // FILTER DATE
        // =========================
        private DateTime GetFromDate(string filter)
        {
            return filter switch
            {
                "today" => DateTime.Today,
                "week" => DateTime.Today.AddDays(-7),
                _ => DateTime.Today.AddMonths(-1)
            };
        }

        // =========================
        // CHART LABELS
        // =========================
        private List<string> GetLabels(string filter)
        {
            return filter switch
            {
                "today" => Enumerable.Range(0, 6)
                    .Select(i => DateTime.Now.AddHours(-i).ToString("HH:00"))
                    .Reverse()
                    .ToList(),

                "week" => Enumerable.Range(0, 7)
                    .Select(i => DateTime.Today.AddDays(-i).ToString("ddd"))
                    .Reverse()
                    .ToList(),

                _ => Enumerable.Range(0, 6)
                    .Select(i => DateTime.Today.AddDays(-i * 5).ToString("dd MMM"))
                    .Reverse()
                    .ToList()
            };
        }

    }
}