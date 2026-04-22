using FanaCRM.Data;
using FanaCRM.Models;
using FanaCRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Controllers
{
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        // ===================== INDEX =====================
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Contacts
                .Include(c => c.Company)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.FullName.Contains(search));
            }

            var contacts = await query
                .OrderByDescending(c => c.CreatedDate)
                .Select(c => new ContactIndexVM
                {
                    Id = c.Id,
                    FullName = c.FullName,
                    Email = c.Email,
                    Phone = c.Phone,
                    Position = c.Position,
                    CompanyName = c.Company.Name,
                    CreatedDate = c.CreatedDate
                })
                .ToListAsync();

            return View(contacts);
        }

        // ===================== CREATE =====================
        public async Task<IActionResult> Create()
        {
            var vm = new ContactCreateVM
            {
                Companies = await GetCompanies()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ContactCreateVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Companies = await GetCompanies();
                return View(vm);
            }

            var contact = new Contact
            {
                FullName = vm.FullName,
                Email = vm.Email,
                Phone = vm.Phone,
                Position = vm.Position,
                CompanyId = vm.CompanyId,
                CreatedDate = DateTime.Now
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===================== EDIT =====================
        public async Task<IActionResult> Edit(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
                return NotFound();

            var vm = new ContactEditVM
            {
                Id = contact.Id,
                FullName = contact.FullName,
                Email = contact.Email,
                Phone = contact.Phone,
                Position = contact.Position,
                CompanyId = contact.CompanyId,
                Companies = await GetCompanies()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ContactEditVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Companies = await GetCompanies();
                return View(vm);
            }

            var contact = await _context.Contacts.FindAsync(vm.Id);

            if (contact == null)
                return NotFound();

            contact.FullName = vm.FullName;
            contact.Email = vm.Email;
            contact.Phone = vm.Phone;
            contact.Position = vm.Position;
            contact.CompanyId = vm.CompanyId;

            _context.Contacts.Update(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===================== DETAILS =====================
        public async Task<IActionResult> Details(int id)
        {
            var contact = await _context.Contacts
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                return NotFound();

            var vm = new ContactEditVM
            {
                Id = contact.Id,
                FullName = contact.FullName,
                Email = contact.Email,
                Phone = contact.Phone,
                Position = contact.Position,
                CompanyId = contact.CompanyId,
                Companies = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Value = contact.CompanyId.ToString(),
                        Text = contact.Company.Name
                    }
                }
            };

            return View(vm);
        }

        // ===================== DELETE =====================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var contact = await _context.Contacts.FindAsync(id);

            if (contact == null)
                return NotFound();

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ===================== HELPER =====================
        private async Task<List<SelectListItem>> GetCompanies()
        {
            return await _context.Companies
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
                .ToListAsync();
        }
    }
}