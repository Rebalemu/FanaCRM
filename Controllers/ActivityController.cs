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
        // INDEX (List + Filter)
        // =========================
        public async Task<IActionResult> Index(string search, string status)
        {
            var query = _context.Activities
                .Include(a => a.Type)
                .Include(a => a.Company)
                .Include(a => a.Contact)
                .Include(a => a.User)
                .AsQueryable();

            // Search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(a => a.Subject.Contains(search));
            }

            // Status filter
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
                ContactName = activity.Contact != null ? activity.Contact.FullName : null,
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
        // CREATE (POST)
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
                AssignedTo = vm.AssignedTo,
                DueDate = vm.DueDate,
                Status = vm.Status,
                CreatedDate = DateTime.UtcNow
            };

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

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
            activity.AssignedTo = vm.AssignedTo;
            activity.DueDate = vm.DueDate;
            activity.Status = vm.Status;

            await _context.SaveChangesAsync();

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
        // DROPDOWN LOADER (REUSABLE)
        // =========================
        private async Task LoadDropdowns(dynamic vm)
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
        }
    }
}