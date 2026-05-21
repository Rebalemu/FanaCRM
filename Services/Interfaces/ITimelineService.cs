using FanaCRM.Models;
using FanaCRM.ViewModels;

namespace FanaCRM.Services.Interfaces
{
    public interface ITimelineService
    {
        Task AddEventAsync(
            string title,
            string? description,
            string eventType,
            string userId,
            int? leadId = null,
            int? opportunityId = null,
            int? activityId = null);

        Task<List<TimelineEvent>> GetLeadTimelineAsync(int leadId);

        Task<List<TimelineEventVM>> GetOpportunityTimelineAsync(int opportunityId);
    }
}