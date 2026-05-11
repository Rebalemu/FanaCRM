using FanaCRM.Data;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Services
{
    public class LeadAutomationService
    {
        private readonly AppDbContext _context;

        public LeadAutomationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task MarkStaleLeads(int staleAfterDays = 7)
        {
            var cutoff = DateTime.UtcNow.AddDays(-staleAfterDays);

            var staleStatus = await _context.LeadStatuses
                .FirstOrDefaultAsync(s => s.Name == "Stale");

            if (staleStatus == null)
                return;

            var leads = await _context.Leads
                .Where(l =>
                    (l.LastContactedDate == null || l.LastContactedDate < cutoff)
                    && l.StatusId != staleStatus.Id
                )
                .ToListAsync();

            foreach (var lead in leads)
            {
                lead.StatusId = staleStatus.Id;
            }

            await _context.SaveChangesAsync();
        }
    }
}