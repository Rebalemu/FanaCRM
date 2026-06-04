using FanaCRM.Data;
using FanaCRM.Models;
using FanaCRM.Services.Interfaces;
using FanaCRM.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Services
{
    public class ActivityService : IActivityService
    {
        private readonly AppDbContext _context;
        private readonly ITimelineService _timelineService;

        public ActivityService(
            AppDbContext context,
            ITimelineService timelineService)
        {
            _context = context;
            _timelineService = timelineService;
        }

        // =========================================================
        // INDEX
        // =========================================================

        public async Task<List<ActivityIndexVM>> GetAllAsync()
        {
            return await _context.Activities
                .Include(a => a.ActivityType)
                .Include(a => a.ActivityStatus)
                .Include(a => a.Company)
                .Include(a => a.Contact)
                .Include(a => a.Lead)
                .Include(a => a.Opportunity)
                .Include(a => a.AssignedTo)
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new ActivityIndexVM
                {
                    Id = a.Id,
                    Subject = a.Subject,

                    TypeName = a.ActivityType.Name,

                    CompanyName = a.Company != null
                        ? a.Company.Name
                        : null,

                    ContactName = a.Contact != null
                        ? a.Contact.FullName
                        : null,

                    LeadName = a.Lead != null
                        ? a.Lead.FullName
                        : null,

                    OpportunityName = a.Opportunity != null
                        ? a.Opportunity.Name
                        : null,

                    AssignedTo = a.AssignedTo.FullName,

                    DueDate = a.DueDate,

                    Status = a.ActivityStatus.Name,

                    IsCompleted = a.IsCompleted
                })
                .ToListAsync();
        }

        // =========================================================
        // DETAILS
        // =========================================================

        public async Task<ActivityDetailsVM?> GetDetailsAsync(int id)
        {
            return await _context.Activities
                .Include(a => a.ActivityType)
                .Include(a => a.ActivityStatus)
                .Include(a => a.Company)
                .Include(a => a.Contact)
                .Include(a => a.Lead)
                .Include(a => a.Opportunity)
                .Include(a => a.AssignedTo)
                .Where(a => a.Id == id)
                .Select(a => new ActivityDetailsVM
                {
                    Id = a.Id,

                    Subject = a.Subject,

                    Description = a.Description,

                    TypeName = a.ActivityType.Name,

                    CompanyName = a.Company != null
                        ? a.Company.Name
                        : null,

                    ContactName = a.Contact != null
                        ? a.Contact.FullName
                        : null,

                    LeadName = a.Lead != null
                        ? a.Lead.FullName
                        : null,

                    OpportunityName = a.Opportunity != null
                        ? a.Opportunity.Name
                        : null,

                    AssignedTo = a.AssignedTo.FullName,

                    DueDate = a.DueDate,

                    Status = a.ActivityStatus.Name,

                    IsCompleted = a.IsCompleted,

                    CreatedDate = a.CreatedAt,

                    CompletedAt = a.CompletedAt
                })
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // CREATE VM
        // =========================================================

        public async Task<ActivityFormVM> GetCreateVMAsync()
        {
            var vm = new ActivityFormVM();

            await LoadDropdowns(vm);

            var openStatus = await _context.ActivityStatuses
                .FirstOrDefaultAsync(x => x.Name == "Open");

            if (openStatus != null)
            {
                vm.StatusId = openStatus.Id;
            }

            return vm;
        }

        // =========================================================
        // CREATE
        // =========================================================

        public async Task CreateAsync(
            ActivityFormVM vm,
            string userId)
        {
            // =====================================
            // BLOCK ACTIVITIES ON CONVERTED LEADS
            // =====================================

            if (vm.LeadId.HasValue)
            {
                var lead = await _context.Leads
                    .FirstOrDefaultAsync(x => x.Id == vm.LeadId);

                if (lead != null && lead.IsConverted)
                {
                    throw new Exception(
                        "Cannot create activity for converted lead.");
                }
            }

            var activity = new Activity
            {
                Subject = vm.Subject,

                Description = vm.Description,

                ActivityTypeId = vm.TypeId,

                ActivityStatusId = vm.StatusId,

                CompanyId = vm.CompanyId,

                ContactId = vm.ContactId,

                LeadId = vm.LeadId,

                OpportunityId = vm.OpportunityId,

                AssignedToId = vm.AssignedToId,

                DueDate = vm.DueDate,

                CreatedById = userId,

                CreatedAt = DateTime.UtcNow,

                IsCompleted = false
            };

            _context.Activities.Add(activity);

            await _context.SaveChangesAsync();

            // =====================================
            // TIMELINE
            // =====================================

            await _timelineService.AddEventAsync(
                title: "Activity Created",

                description:
                    $"{activity.Subject} scheduled.",

                eventType: "Activity",

                userId: userId,

                leadId: activity.LeadId,

                opportunityId: activity.OpportunityId,

                activityId: activity.Id
            );
        }

        // =========================================================
        // EDIT VM
        // =========================================================

        public async Task<ActivityFormVM?> GetEditVMAsync(int id)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return null;

            var vm = new ActivityFormVM
            {
                Id = activity.Id,

                Subject = activity.Subject,

                Description = activity.Description,

                TypeId = activity.ActivityTypeId,

                StatusId = activity.ActivityStatusId,

                CompanyId = activity.CompanyId,

                ContactId = activity.ContactId,

                LeadId = activity.LeadId,

                OpportunityId = activity.OpportunityId,

                AssignedToId = activity.AssignedToId,

                DueDate = activity.DueDate
            };

            await LoadDropdowns(vm);

            return vm;
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateAsync(
            ActivityFormVM vm,
            string userId)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == vm.Id);

            if (activity == null)
                return;

            // =====================================
            // BLOCK CONVERTED LEADS
            // =====================================

            if (vm.LeadId.HasValue)
            {
                var lead = await _context.Leads
                    .FirstOrDefaultAsync(x => x.Id == vm.LeadId);

                if (lead != null && lead.IsConverted)
                {
                    throw new Exception(
                        "Converted leads are read-only.");
                }
            }

            activity.Subject = vm.Subject;

            activity.Description = vm.Description;

            activity.ActivityTypeId = vm.TypeId;

            activity.ActivityStatusId = vm.StatusId;

            activity.CompanyId = vm.CompanyId;

            activity.ContactId = vm.ContactId;

            activity.LeadId = vm.LeadId;

            activity.OpportunityId = vm.OpportunityId;

            activity.AssignedToId = vm.AssignedToId;

            activity.DueDate = vm.DueDate;

            activity.UpdatedAt = DateTime.UtcNow;

            activity.UpdatedById = userId;

            await _context.SaveChangesAsync();

            // =====================================
            // TIMELINE
            // =====================================

            await _timelineService.AddEventAsync(
                title: "Activity Updated",

                description:
                    $"{activity.Subject} updated.",

                eventType: "Activity",

                userId: userId,

                leadId: activity.LeadId,

                opportunityId: activity.OpportunityId,

                activityId: activity.Id
            );
        }

        // =========================================================
        // COMPLETE
        // =========================================================

        public async Task CompleteAsync(
            int id,
            string userId)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return;

            var completedStatusId =
                await GetStatusIdAsync("Completed");

            activity.IsCompleted = true;

            activity.CompletedAt = DateTime.UtcNow;

            activity.ActivityStatusId = completedStatusId;

            await _context.SaveChangesAsync();

            // =====================================
            // TIMELINE
            // =====================================

            await _timelineService.AddEventAsync(
                title: "Activity Completed",

                description:
                    $"{activity.Subject} completed.",

                eventType: "Activity",

                userId: userId,

                leadId: activity.LeadId,

                opportunityId: activity.OpportunityId,

                activityId: activity.Id
            );
        }

        // =========================================================
        // CANCEL
        // =========================================================

        public async Task CancelAsync(
            int id,
            string userId)
        {
            var activity = await _context.Activities
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null)
                return;

            var cancelledStatusId =
                await GetStatusIdAsync("Cancelled");

            activity.ActivityStatusId = cancelledStatusId;

            await _context.SaveChangesAsync();

            // =====================================
            // TIMELINE
            // =====================================

            await _timelineService.AddEventAsync(
                title: "Activity Cancelled",

                description:
                    $"{activity.Subject} cancelled.",

                eventType: "Activity",

                userId: userId,

                leadId: activity.LeadId,

                opportunityId: activity.OpportunityId,

                activityId: activity.Id
            );
        }

        // =========================================================
        // STATUS HELPER
        // =========================================================

        private async Task<int> GetStatusIdAsync(string name)
        {
            return await _context.ActivityStatuses
                .Where(x => x.Name == name)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();
        }

        // =========================================================
        // LOAD DROPDOWNS
        // =========================================================

        public async Task LoadDropdowns(ActivityFormVM vm)
        {
            vm.Types = await _context.ActivityTypes
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();

            vm.Statuses = await _context.ActivityStatuses
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();

            vm.Companies = await _context.Companies
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();

            vm.Contacts = await _context.Contacts
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToListAsync();

            // ONLY NON-CONVERTED LEADS

            vm.Leads = await _context.Leads
                .Where(x => !x.IsConverted)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.FullName
                })
                .ToListAsync();

            vm.Opportunities = await _context.Opportunities
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
                .ToListAsync();

            vm.Users = await _context.Users
                .Select(x => new SelectListItem
                {
                    Value = x.Id,
                    Text = x.FullName
                })
                .ToListAsync();
        }

        // =========================================================
        // DASHBOARD
        // =========================================================

        public async Task<ActivityDashboardVM> GetDashboardAsync(string userId)
        {
            var today = DateTime.Now.Date;

            var activities = await _context.Activities
                .AsNoTracking()
                .Include(a => a.ActivityType)
                .Include(a => a.ActivityStatus)
                .Include(a => a.Company)
                .Include(a => a.Contact)
                .Include(a => a.Lead)
                .Include(a => a.Opportunity)
                .Include(a => a.AssignedTo)

                // REMOVE THIS TEMPORARILY
                //.Where(a => a.AssignedToId == userId)

                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return new ActivityDashboardVM
            {
                TotalActivities = activities.Count,

                PendingActivities = activities.Count(a =>
                    !a.IsCompleted &&
                    a.ActivityStatus != null &&
                    a.ActivityStatus.Name != "Cancelled"),

                CompletedToday = activities.Count(a =>
                    a.CompletedAt.HasValue &&
                    a.CompletedAt.Value.Date == today),

                OverdueActivities = activities.Count(a =>
                    !a.IsCompleted &&
                    a.DueDate.HasValue &&
                    a.DueDate.Value.Date < today &&
                    a.ActivityStatus != null &&
                    a.ActivityStatus.Name != "Cancelled"),

                MyActivities = activities
                    .Take(10)
                    .Select(MapActivity)
                    .ToList(),

                UpcomingActivities = activities
                    .Where(a =>
                        !a.IsCompleted &&
                        a.DueDate.HasValue &&
                        a.DueDate.Value.Date >= today &&
                        a.ActivityStatus != null &&
                        a.ActivityStatus.Name != "Cancelled")
                    .OrderBy(a => a.DueDate)
                    .Take(10)
                    .Select(MapActivity)
                    .ToList(),

                OverdueList = activities
                    .Where(a =>
                        !a.IsCompleted &&
                        a.DueDate.HasValue &&
                        a.DueDate.Value.Date < today &&
                        a.ActivityStatus != null &&
                        a.ActivityStatus.Name != "Cancelled")
                    .OrderBy(a => a.DueDate)
                    .Take(10)
                    .Select(MapActivity)
                    .ToList(),

                CompletedTodayList = activities
                    .Where(a =>
                        a.CompletedAt.HasValue &&
                        a.CompletedAt.Value.Date == today)
                    .OrderByDescending(a => a.CompletedAt)
                    .Take(10)
                    .Select(MapActivity)
                    .ToList()
            };
        }
        // =========================================================
        // MAP
        // =========================================================

        private static ActivityIndexVM MapActivity(Activity a)
        {
            return new ActivityIndexVM
            {
                Id = a.Id,

                Subject = a.Subject,

                TypeName = a.ActivityType?.Name ?? "-",

                CompanyName = a.Company?.Name,

                ContactName = a.Contact?.FullName,

                LeadName = a.Lead?.FullName,

                OpportunityName = a.Opportunity?.Name,

                AssignedTo = a.AssignedTo?.FullName ?? "-",

                DueDate = a.DueDate,

                Status = a.ActivityStatus?.Name ?? "-",

                IsCompleted = a.IsCompleted
            };
        }
    }
}