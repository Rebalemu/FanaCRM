using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FanaCRM.Data;
using FanaCRM.Models;
using FanaCRM.ViewModels;

namespace FanaCRM.Controllers
{
    public class ActivityController : Controller
    {
        private readonly AppDbContext _context;

        public ActivityController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index(string search, string status)
        {
            var query = _context.Activities
                .Include(a => a.Type)
                .Include(a => a.Company)
                .Include(a => a.Contact)
                .Include(a => a.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Subject.Contains(search));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(a => a.Status == status);
            }

            var activities = await query
                .OrderByDescending(a => a.CreatedDate)
                .Select(a => new ActivityIndexVM
                {
                    Id = a.Id,
                    Subject = a.Subject,
                    TypeName = a.Type.Name,
                    CompanyName = a.Company != null ? a.Company.Name : null,
                    ContactName = a.Contact != null ? a.Contact.FullName : null,
                    AssignedTo = a.User.FullName,
                    DueDate = a.DueDate,
                    Status = a.Status
                })
                .AsNoTracking()
                .ToListAsync();

            return View(activities);
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var activity = await _context.Activities
                .Include(a => a.Type)
                .Include(a => a.Company)
                .Include(a => a.Contact)
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (activity == null) return NotFound();

            var vm = new ActivityDetailsVM
            {
                Id = activity.Id,
                Subject = activity.Subject,
                Description = activity.Description,
                TypeName = activity.Type.Name,
                CompanyName = activity.Company?.Name,
                ContactName = activity.Contact?.FullName,
                AssignedTo = activity.User.FullName,
                DueDate = activity.DueDate,
                Status = activity.Status,
                CreatedDate = activity.CreatedDate
            };

            return View(vm);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public async Task<IActionResult> Create()
        {
            var vm = new ActivityFormVM();
            await LoadDropdowns(vm);
            return View(vm);
        }

        // =========================
        // CREATE (POST) ⭐ MAIN LOGIC
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivityFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(vm);
                return View(vm);
            }

            var activity = new Activity
            {
                TypeId = vm.TypeId,
                Subject = vm.Subject,
                Description = vm.Description,
                CompanyId = vm.CompanyId,
                ContactId = vm.ContactId,
                LeadId = vm.Id, // ⭐ IMPORTANT ADDITION
                AssignedTo = vm.AssignedTo,
                DueDate = vm.DueDate,
                Status = vm.Status,
                CreatedDate = DateTime.UtcNow
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            // =========================
            // STEP 4: UPDATE LEAD TRACKING
            // =========================
            await UpdateLeadTracking(activity);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return NotFound();

            var vm = new ActivityFormVM
            {
                Id = activity.Id,
                TypeId = activity.TypeId,
                Subject = activity.Subject,
                Description = activity.Description,
                CompanyId = activity.CompanyId,
                ContactId = activity.ContactId,
                LeadId = activity.LeadId,
                AssignedTo = activity.AssignedTo,
                DueDate = activity.DueDate,
                Status = activity.Status
            };

            await LoadDropdowns(vm);
            return View(vm);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActivityFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadDropdowns(vm);
                return View(vm);
            }

            var activity = await _context.Activities.FindAsync(vm.Id);
            if (activity == null) return NotFound();

            activity.TypeId = vm.TypeId;
            activity.Subject = vm.Subject;
            activity.Description = vm.Description;
            activity.CompanyId = vm.CompanyId;
            activity.ContactId = vm.ContactId;
            activity.LeadId = vm.LeadId;
            activity.AssignedTo = vm.AssignedTo;
            activity.DueDate = vm.DueDate;
            activity.Status = vm.Status;

            await _context.SaveChangesAsync();

            await UpdateLeadTracking(activity); // ⭐ keep tracking updated

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity == null) return NotFound();

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // STEP 4 CORE LOGIC
        // =========================
        private async Task UpdateLeadTracking(Activity activity)
        {
            if (activity.LeadId == null)
                return;

            var lead = await _context.Leads.FindAsync(activity.LeadId);

            if (lead == null)
                return;

            // Always update last activity
            lead.LastActivityDate = activity.CreatedDate;

            // Only real contact counts
            if (IsContactActivity(activity.TypeId))
            {
                lead.LastContactedDate = activity.CreatedDate;
            }

            // Optional: revive stale lead
            var staleStatus = await _context.LeadStatuses
                .FirstOrDefaultAsync(s => s.Name == "Stale");

            var activeStatus = await _context.LeadStatuses
                .FirstOrDefaultAsync(s => s.Name == "New");

            if (staleStatus != null &&
                activeStatus != null &&
                lead.StatusId == staleStatus.Id)
            {
                lead.StatusId = activeStatus.Id;
            }

            await _context.SaveChangesAsync();
        }

        private bool IsContactActivity(int typeId)
        {
            // adjust to your ActivityType IDs
            return typeId == 1 || typeId == 2 || typeId == 3;
        }

        // =========================
        // DROPDOWNS
        // =========================
        private async Task LoadDropdowns(ActivityFormVM vm)
        {
            vm.Types = await _context.ActivityTypes
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = t.Name
                }).ToListAsync();

            vm.Companies = await _context.Companies
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToListAsync();

            vm.Contacts = await _context.Contacts
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.FullName
                }).ToListAsync();

            vm.Users = await _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.FullName
                }).ToListAsync();

            vm.Leads = await _context.Leads
                .Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.FullName
                }).ToListAsync();
        }
    }
}