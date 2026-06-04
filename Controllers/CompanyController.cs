using FanaCRM.Data;
using FanaCRM.Models;
using FanaCRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Controllers
{
    public class CompanyController : Controller
    {
        private readonly AppDbContext _context;

        public CompanyController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index(string search)
        {
            var query = _context.Companies.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                                    (c.Name != null && c.Name.Contains(search)) ||
                                    (c.Industry != null && c.Industry.Contains(search)));
            }

            var companies = await query
                .OrderByDescending(c => c.Id)
                .Select(c => new Company
                {
                    Id = c.Id,
                    Name = c.Name,
                    Industry = c.Industry,
                    Website = c.Website,
                    Phone = c.Phone,
                    Address = c.Address,
                    CreatedDate = c.CreatedDate
                })
                .ToListAsync();

            return View(companies);
        }

        // =========================
        // CREATE - GET
        // =========================
        public IActionResult Create()
        {
            return View();
        }

        // =========================
        // CREATE - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CompanyCreateVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var company = new Company
            {
                Name = vm.Name,
                Industry = vm.Industry,
                Website = vm.Website,
                Phone = vm.Phone,
                Address = vm.Address,
                CreatedDate = DateTime.Now
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT - GET
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
                return NotFound();

            var vm = new CompanyEditVM
            {
                Id = company.Id,
                Name = company.Name,
                Industry = company.Industry,
                Website = company.Website,
                Phone = company.Phone,
                Address = company.Address
            };

            return View(vm);
        }

        // =========================
        // EDIT - POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CompanyEditVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var company = await _context.Companies.FindAsync(vm.Id);

            if (company == null)
                return NotFound();

            company.Name = vm.Name;
            company.Industry = vm.Industry;
            company.Website = vm.Website;
            company.Phone = vm.Phone;
            company.Address = vm.Address;

            _context.Companies.Update(company);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var company = await _context.Companies
                .Include(c => c.Contacts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (company == null)
                return NotFound();

            var vm = new CompanyDetailsVM
            {
                Id = company.Id,
                Name = company.Name,
                Industry = company.Industry,
                Website = company.Website,
                Phone = company.Phone,
                Address = company.Address,
                CreatedDate = company.CreatedDate,
                Contacts = company.Contacts?
                    .Select(c => new ContactIndexVM
                    {
                        Id = c.Id,
                        FullName = c.FullName,
                        Email = c.Email,
                        Phone = c.Phone,
                        Position = c.Position
                    })
                    .ToList() ?? new List<ContactIndexVM>()
            };

            return View(vm);
        }

        // =========================
        // DELETE (for modal)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
                return NotFound();

            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}