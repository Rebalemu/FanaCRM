using FanaCRM.Data;
using FanaCRM.Models;
using FanaCRM.Services.Interfaces;
using FanaCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Controllers
{
    [Authorize(Roles = "Admin,Sales")]
    public class LeadController : Controller
    {

        private readonly AppDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly ILeadService _leadService;
        private readonly ITimelineService _timelineService;

        public LeadController(AppDbContext context, UserManager<Users> userManager, ILeadService leadService, ITimelineService timelineService)
        {
            _context = context;
            _userManager = userManager;
            _leadService = leadService;
            _timelineService = timelineService;
        }
        public async Task<IActionResult> Index(string search, int? statusId)
        {
            var query = _context.Leads
                .Include(l => l.Source)
                .Include(l => l.Status)
                .Include(l => l.User)
                .AsQueryable();

            // 🔐 ROLE-BASED FILTER (Sales only sees their leads)
            if (User.IsInRole("Sales"))
            {
                var userId = _userManager.GetUserId(User);
                query = query.Where(l => l.AssignedTo == userId);
            }

            // 🔎 SEARCH BY NAME
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(l => l.FullName.Contains(search));
            }

            // 🎯 FILTER BY STATUS
            if (statusId.HasValue)
            {
                query = query.Where(l => l.StatusId == statusId);
            }

            var leads = await query
                .Select(l => new LeadIndexVM
                {
                    Id = l.Id,
                    FullName = l.FullName,
                    Email = l.Email,
                    Phone = l.Phone,
                    Company = l.Company,

                    Source = l.Source.Name,
                    Status = l.Status.Name,

                    AssignedTo = l.User != null ? l.User.FullName : "Unassigned",

                    CreatedDate = l.CreatedDate
                })
                .ToListAsync();

            return View(leads);
        }

        // GET create
        public async Task<IActionResult> Create()
        {
            var vm = new LeadCreateVM
            {
                Sources = await _context.LeadSources
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToListAsync(),

                Statuses = await _context.LeadStatuses
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToListAsync(),

                Users = await _userManager.Users
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.FullName
                    }).ToListAsync()
            };

            return View(vm);
        }
        // POST Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeadCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                // 🔥 RELOAD DROPDOWNS (VERY IMPORTANT)
                vm.Sources = await _context.LeadSources
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                    .ToListAsync();

                vm.Statuses = await _context.LeadStatuses
                    .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name })
                    .ToListAsync();

                vm.Users = await _userManager.Users
                    .Select(x => new SelectListItem { Value = x.Id, Text = x.FullName })
                    .ToListAsync();

                return View(vm);
            }

            var lead = new Lead
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Phone = vm.Phone,
                Company = vm.Company,

                SourceId = vm.SourceId,
                StatusId = vm.StatusId == 0 ? 1 : vm.StatusId, // default = New

                AssignedTo = string.IsNullOrEmpty(vm.AssignedTo) ? null : vm.AssignedTo,

                CreatedDate = DateTime.Now
            };
            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();
            var userId = _userManager.GetUserId(User);

            await _timelineService.AddEventAsync(
                title: "Lead Created",
                description: $"{lead.FullName} was created",
                eventType: "Lead",
                userId: userId,
                leadId: lead.Id
            );

            return RedirectToAction("Index");
        }
        // GET: Lead/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var lead = await _context.Leads.FindAsync(id);
            if (lead == null)
                return NotFound();

            var vm = new LeadEditVM
            {
                Id = lead.Id,
                FullName = lead.FullName,
                Email = lead.Email,
                Phone = lead.Phone,
                Company = lead.Company,
                SourceId = lead.SourceId,
                StatusId = lead.StatusId,
                AssignedTo = lead.AssignedTo,

                Sources = await _context.LeadSources
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToListAsync(),

                Statuses = await _context.LeadStatuses
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToListAsync(),

                Users = await _userManager.Users
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.FullName
                    }).ToListAsync()
            };

            return View(vm);
        }
                [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(LeadEditVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Sources = await _context.LeadSources
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToListAsync();

                vm.Statuses = await _context.LeadStatuses
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToListAsync();

                vm.Users = await _userManager.Users
                    .Select(x => new SelectListItem
                    {
                        Value = x.Id,
                        Text = x.FullName
                    }).ToListAsync();

                return View(vm);
            }

            var lead = await _context.Leads
                .Include(l => l.Status)
                .Include(l => l.Source)
                .FirstOrDefaultAsync(l => l.Id == vm.Id);

            if (lead == null)
                return NotFound();

            var userId = _userManager.GetUserId(User);

            // =========================================
            // TRACK CHANGES
            // =========================================

            var changes = new List<string>();

            // FULL NAME
            if (lead.FullName != vm.FullName)
            {
                changes.Add(
                    $"Full Name changed from '{lead.FullName}' to '{vm.FullName}'");

                lead.FullName = vm.FullName;
            }

            // EMAIL
            if (lead.Email != vm.Email)
            {
                changes.Add(
                    $"Email changed from '{lead.Email}' to '{vm.Email}'");

                lead.Email = vm.Email;
            }

            // PHONE
            if (lead.Phone != vm.Phone)
            {
                changes.Add(
                    $"Phone changed from '{lead.Phone}' to '{vm.Phone}'");

                lead.Phone = vm.Phone;
            }

            // COMPANY
            if (lead.Company != vm.Company)
            {
                changes.Add(
                    $"Company changed from '{lead.Company}' to '{vm.Company}'");

                lead.Company = vm.Company;
            }

            // STATUS
            if (lead.StatusId != vm.StatusId)
            {
                var oldStatus = lead.Status?.Name;

                var newStatus = await _context.LeadStatuses
                    .Where(s => s.Id == vm.StatusId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync();

                changes.Add(
                    $"Status changed from '{oldStatus}' to '{newStatus}'");

                lead.StatusId = vm.StatusId;
            }

            // SOURCE
            if (lead.SourceId != vm.SourceId)
            {
                var oldSource = lead.Source?.Name;

                var newSource = await _context.LeadSources
                    .Where(s => s.Id == vm.SourceId)
                    .Select(s => s.Name)
                    .FirstOrDefaultAsync();

                changes.Add(
                    $"Source changed from '{oldSource}' to '{newSource}'");

                lead.SourceId = vm.SourceId;
            }

            // ASSIGNED USER
            if (lead.AssignedTo != vm.AssignedTo)
            {
                changes.Add("Assigned user changed");

                lead.AssignedTo = vm.AssignedTo;
            }

            // SAVE LEAD
            await _context.SaveChangesAsync();

            // =========================================
            // CREATE TIMELINE EVENTS
            // =========================================

            foreach (var change in changes)
            {
                await _timelineService.AddEventAsync(
                    title: "Lead Updated",
                    description: change,
                    eventType: "Lead",
                    userId: userId,
                    leadId: lead.Id
                );
            }

            TempData["Success"] = "Lead updated successfully";

            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var lead = await _context.Leads.FindAsync(id);
            if (lead == null)
                return NotFound();

            _context.Leads.Remove(lead);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Lead deleted successfully";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Convert(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);

                var opportunityId =
                    await _leadService.ConvertLeadAsync(id, userId);

                return RedirectToAction("Details", "Opportunity", new { id = opportunityId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction("Index");
            }
        }
                public async Task<IActionResult> Details(int id)
        {
            var lead = await _context.Leads
                .Include(l => l.Source)
                .Include(l => l.Status)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lead == null)
                return NotFound();

            // =========================================
            // TIMELINE
            // =========================================

            var timeline = await _timelineService
                .GetLeadTimelineAsync(id);

            // =========================================
            // UPCOMING ACTIVITIES
            // =========================================

            var activities = await _context.Activities
                .Include(a => a.ActivityType)
                .Include(a => a.ActivityStatus)
                .Where(a =>
                    a.LeadId == id &&
                    !a.IsCompleted)
                .OrderBy(a => a.DueDate)
                .Take(5)
                .Select(a => new ActivityWidgetVM
                {
                    Id = a.Id,

                    Subject = a.Subject,

                    Type = a.ActivityType.Name,

                    Status = a.ActivityStatus.Name,

                    DueDate = a.DueDate,

                    IsOverdue =
                        a.DueDate.HasValue &&
                        a.DueDate < DateTime.Now &&
                        !a.IsCompleted
                })
                .ToListAsync();

            // =========================================
            // VIEWMODEL
            // =========================================

            var vm = new LeadDetailsVM
            {
                Id = lead.Id,

                FullName = lead.FullName,

                Email = lead.Email,

                Phone = lead.Phone,

                Company = lead.Company,

                Status = lead.Status.Name,

                Source = lead.Source.Name,

                AssignedTo = lead.User != null
                    ? lead.User.FullName
                    : "Unassigned",

                CreatedDate = lead.CreatedDate,

                Timeline = timeline.Select(t => new TimelineEventVM
                {
                    Title = t.Title,

                    Description = t.Description,

                    EventType = t.EventType,

                    UserName = t.User.FullName,

                    CreatedDate = t.CreatedDate
                }).ToList(),

                UpcomingActivities = activities
            };

            return View(vm);
        }
    }
}