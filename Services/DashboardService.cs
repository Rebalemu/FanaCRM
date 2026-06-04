using FanaCRM.ViewModels;
using FanaCRM.Data;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Services
{
    public interface IDashboardService
    {
        Task<DashboardVM> GetDashboardDataAsync(string filter);
    }

    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;

        public DashboardService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardVM> GetDashboardDataAsync(string filter = "month")
        {
            var today = DateTime.Today;

            DateTime startDate = filter switch
            {
                "today" => today,
                "week" => today.AddDays(-(int)today.DayOfWeek),
                "month" => new DateTime(today.Year, today.Month, 1),
                _ => today.AddMonths(-1)
            };

            const int ConvertedStatusId = 3;
            const int WonStageId = 5;

            var leadsQuery = _context.Leads
                .Where(l => l.CreatedDate.Date >= startDate);

            var opportunitiesQuery = _context.Opportunities
                .Where(o =>
                    o.CreatedDate.Date >= startDate &&
                    o.StageId == WonStageId
                );

            var totalLeads = await leadsQuery.CountAsync();

            var convertedLeads = await leadsQuery
                .CountAsync(l => l.StatusId == ConvertedStatusId);

            var conversionRate = totalLeads == 0
                ? 0
                : (double)convertedLeads / totalLeads * 100;

            // GROUPING
            var groupedLeads = await leadsQuery
                .GroupBy(l => l.CreatedDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();

            var groupedRevenue = await opportunitiesQuery
                .GroupBy(o => o.CreatedDate.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    Revenue = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            // FULL DATE RANGE (fix missing dates issue)
            var days = Enumerable.Range(0, (today - startDate).Days + 1)
                .Select(d => startDate.AddDays(d))
                .ToList();

            var labels = days.Select(d => d.ToString("MM-dd")).ToList();

            var leadsData = days.Select(d =>
                groupedLeads.FirstOrDefault(x => x.Date == d)?.Count ?? 0
            ).ToList();

            var revenueData = days.Select(d =>
                groupedRevenue.FirstOrDefault(x => x.Date == d)?.Revenue ?? 0
            ).ToList();

            return new DashboardVM
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalContacts = await _context.Contacts.CountAsync(),
                TotalCompanies = await _context.Companies.CountAsync(),

                TotalLeads = totalLeads,
                ConvertedLeads = convertedLeads,
                ConversionRate = conversionRate,

                Labels = labels,
                LeadsData = leadsData,
                RevenueData = revenueData,

                Filter = filter
            };
        }
    }
}