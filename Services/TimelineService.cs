using FanaCRM.Data;
using FanaCRM.Models;
using FanaCRM.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Services
{
    public class TimelineService : ITimelineService
    {
        private readonly AppDbContext _context;

        public TimelineService(AppDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // ADD EVENT
        // =====================================================

        public async Task AddEventAsync(
            string title,
            string? description,
            string eventType,
            string userId,
            int? leadId = null,
            int? opportunityId = null,
            int? activityId = null)
        {
            var timeline = new TimelineEvent
            {
                Title = title,

                Description = description,

                EventType = eventType,

                UserId = userId,

                LeadId = leadId,

                OpportunityId = opportunityId,

                ActivityId = activityId,

                CreatedDate = DateTime.UtcNow
            };

            _context.TimelineEvents.Add(timeline);

            await _context.SaveChangesAsync();
        }

        // =====================================================
        // LEAD TIMELINE
        // =====================================================

        public async Task<List<TimelineEvent>> GetLeadTimelineAsync(int leadId)
        {
            return await _context.TimelineEvents
                .Include(t => t.User)
                .Include(t => t.Activity)
                .Where(t => t.LeadId == leadId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        // =====================================================
        // OPPORTUNITY TIMELINE
        // =====================================================

        public async Task<List<TimelineEvent>> GetOpportunityTimelineAsync(int opportunityId)
        {
            return await _context.TimelineEvents
                .Include(t => t.User)
                .Include(t => t.Activity)
                .Where(t => t.OpportunityId == opportunityId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }
}