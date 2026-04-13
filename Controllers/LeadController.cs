using FanaCRM.Data;
using FanaCRM.Models;
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

        public LeadController(AppDbContext context, UserManager<Users> userManager)
        {
            _context = context;
            _userManager = userManager;
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
                // reload dropdowns
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

            var lead = await _context.Leads.FindAsync(vm.Id);
            if (lead == null)
                return NotFound();

            // update fields
            lead.FullName = vm.FullName;
            lead.Email = vm.Email;
            lead.Phone = vm.Phone;
            lead.Company = vm.Company;
            lead.SourceId = vm.SourceId;
            lead.StatusId = vm.StatusId;
            lead.AssignedTo = string.IsNullOrEmpty(vm.AssignedTo) ? null : vm.AssignedTo;

            _context.Update(lead);
            await _context.SaveChangesAsync();

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

    }
}