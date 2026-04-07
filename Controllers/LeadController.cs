using FanaCRM.Data;
using FanaCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Controllers
{
    [Authorize(Roles = "Admin,Sales")]
    public class LeadController : Controller
    {
        private readonly AppDbContext _context;

        public LeadController(AppDbContext context)
        {
            _context = context;
        }

        // ✅ INDEX + FILTER BY STATUS
        public async Task<IActionResult> Index(int? statusId)
        {
            var leads = _context.Leads
                .Include(l => l.Source)
                .Include(l => l.Status)
                .Include(l => l.User)
                .AsQueryable();

            if (statusId.HasValue)
            {
                leads = leads.Where(l => l.StatusId == statusId);
            }

            ViewBag.StatusList = new SelectList(_context.LeadStatuses, "Id", "Name");

            return View(await leads.ToListAsync());
        }

        // ✅ GET: CREATE
        public IActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        // ✅ POST: CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Lead lead)
        {
            if (ModelState.IsValid)
            {
                lead.CreatedDate = DateTime.Now;

                _context.Leads.Add(lead);
                await _context.SaveChangesAsync();
                TempData["success"] = "Category created successfully";
                return RedirectToAction(nameof(Index));
            }
            LoadDropdowns();

            return View(lead);
        }

        // ✅ GET: EDIT
        public async Task<IActionResult> Edit(int id)
        {
            var lead = await _context.Leads.FindAsync(id);
            if (lead == null) return NotFound();

            LoadDropdowns();
            return View(lead);
        }

        // ✅ POST: EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Lead lead)
        {
            if (id != lead.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(lead);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Leads.Any(e => e.Id == lead.Id))
                        return NotFound();
                    else
                        throw;
                }
                TempData["success"] = "Category updated successfully";
                return RedirectToAction(nameof(Index));
            }

            LoadDropdowns();
            return View(lead);
        }

        // ✅ DELETE
        public async Task<IActionResult> Delete(int id)
        {
            var lead = await _context.Leads
                .Include(l => l.Source)
                .Include(l => l.Status)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (lead == null) return NotFound();

            return View(lead);
        }

        // ✅ POST: DELETE CONFIRMED
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lead = await _context.Leads.FindAsync(id);

            if (lead != null)
            {
                _context.Leads.Remove(lead);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // ✅ CHANGE STATUS QUICK ACTION
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(int id, int statusId)
        {
            var lead = await _context.Leads.FindAsync(id);

            if (lead == null) return NotFound();

            lead.StatusId = statusId;
            await _context.SaveChangesAsync();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }

        // ✅ CONVERT LEAD → CUSTOMER (Basic Logic)
        // public async Task<IActionResult> ConvertToCustomer(int id)
        // {
        //     var lead = await _context.Leads.FindAsync(id);

        //     if (lead == null) return NotFound();

        //     // 🔹 Example Customer Creation (you must create Customer model/table)
        //     var customer = new Customer
        //     {
        //         FullName = lead.FullName,
        //         Email = lead.Email,
        //         Phone = lead.Phone,
        //         Company = lead.Company,
        //         CreatedDate = DateTime.Now
        //     };

        //     _context.Customers.Add(customer);

        //     // Optional: update lead status to "Converted"
        //     var convertedStatus = await _context.LeadStatuses
        //         .FirstOrDefaultAsync(s => s.Name == "Converted");

        //     if (convertedStatus != null)
        //     {
        //         lead.StatusId = convertedStatus.Id;
        //     }

        //     await _context.SaveChangesAsync();

        //     return RedirectToAction(nameof(Index));
        // }

        // ✅ LOAD DROPDOWNS (REUSABLE)
        private void LoadDropdowns()
        {
            ViewBag.SourceId = new SelectList(_context.LeadSources, "Id", "Name");
            ViewBag.StatusId = new SelectList(_context.LeadStatuses, "Id", "Name");
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
        }
    }
}