using FanaCRM.Data;
using FanaCRM.Models;
using FanaCRM.Services.Interfaces;
using FanaCRM.ViewModels;
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


        public async Task<List<TimelineEventVM>>
    GetOpportunityTimelineAsync(int opportunityId)
        {
            var opportunity = await _context.Opportunities
                .AsNoTracking()
                .FirstOrDefaultAsync(o => o.Id == opportunityId);

            if (opportunity == null)
                return new();

            return await _context.TimelineEvents
                .AsNoTracking()
                .Include(t => t.User)
                .Where(t =>

                    t.OpportunityId == opportunityId ||

                    (opportunity.LeadId != null &&
                     t.LeadId == opportunity.LeadId)
                )
                .OrderByDescending(t => t.CreatedDate)
                .Select(t => new TimelineEventVM
                {
                    Id = t.Id,
                    Title = t.Title,
                    Description = t.Description,
                    CreatedDate = t.CreatedDate,
                    EventType = t.EventType,
                    UserName = t.User.FullName
                })
                .ToListAsync();
        }
    }
}