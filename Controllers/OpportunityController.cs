using FanaCRM.Data;
using FanaCRM.Models;
using FanaCRM.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace FanaCRM.Controllers
{
    public class OpportunityController : Controller
    {
        private readonly AppDbContext _context;

        public OpportunityController(AppDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index(string search, int? stageId)
        {
            var query = _context.Opportunities
                .Include(o => o.Company)
                .Include(o => o.Stage)
                .Include(o => o.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(o => o.Name.Contains(search));

            if (stageId.HasValue)
                query = query.Where(o => o.StageId == stageId);

            var data = await query
                .Select(o => new OpportunityIndexVM
                {
                    Id = o.Id,
                    Name = o.Name,
                    CompanyName = o.Company.Name,
                    StageName = o.Stage.Name,
                    Probability = o.Stage.Probability,
                    Amount = o.Amount,
                    CloseDate = o.CloseDate,
                    AssignedTo = o.User.FullName
                })
                .ToListAsync();

            ViewBag.Stages = await _context.OpportunityStages
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                })
                .ToListAsync();

            return View(data);
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var opportunity = await _context.Opportunities
                .Include(o => o.Company)
                .Include(o => o.Contact)
                .Include(o => o.Stage)
                .Include(o => o.User)
                .Include(o => o.Products)
                    .ThenInclude(p => p.Product)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return NotFound();

            var vm = new OpportunityDetailsVM
            {
                Id = opportunity.Id,
                Name = opportunity.Name,
                CompanyName = opportunity.Company.Name,
                ContactName = opportunity.Contact?.FullName,
                StageName = opportunity.Stage.Name,
                Probability = opportunity.Stage.Probability,
                Amount = opportunity.Amount,
                CloseDate = opportunity.CloseDate,
                AssignedTo = opportunity.User.FullName,
                Products = opportunity.Products.Select(p => new OpportunityProductVM
                {
                    ProductId = p.ProductId,
                    ProductName = p.Product.Name,
                    Quantity = p.Quantity,
                    Price = p.Price
                }).ToList()
            };

            return View(vm);
        }

        // =========================
        // CREATE (GET)
        // =========================
        public IActionResult Create()
        {
            var vm = BuildFormVM(new OpportunityFormVM());
            return View(vm);
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OpportunityFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm = BuildFormVM(vm);
                return View(vm);
            }

            var opportunity = new Opportunity
            {
                CompanyId = vm.CompanyId,
                ContactId = vm.ContactId,
                Name = vm.Name,
                StageId = vm.StageId,
                CloseDate = vm.CloseDate,
                AssignedTo = vm.AssignedTo,
                CreatedDate = DateTime.UtcNow
            };

            _context.Opportunities.Add(opportunity);
            await _context.SaveChangesAsync();

            foreach (var p in vm.Products)
            {
                _context.OpportunityProducts.Add(new OpportunityProduct
                {
                    OpportunityId = opportunity.Id,
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    Price = p.Price
                });
            }

            await _context.SaveChangesAsync();

            opportunity.Amount = await _context.OpportunityProducts
                .Where(x => x.OpportunityId == opportunity.Id)
                .SumAsync(x => x.Quantity * x.Price);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT (GET)
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var opportunity = await _context.Opportunities
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return NotFound();

            var vm = new OpportunityFormVM
            {
                Id = opportunity.Id,
                CompanyId = opportunity.CompanyId,
                ContactId = opportunity.ContactId,
                Name = opportunity.Name,
                StageId = opportunity.StageId,
                CloseDate = opportunity.CloseDate,
                AssignedTo = opportunity.AssignedTo,

                Products = opportunity.Products.Select(p => new OpportunityProductVM
                {
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    Price = p.Price
                }).ToList()
            };

            vm = BuildFormVM(vm);
            return View(vm);
        }

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OpportunityFormVM vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                vm = BuildFormVM(vm);
                return View(vm);
            }

            var opportunity = await _context.Opportunities
                .Include(o => o.Products)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (opportunity == null)
                return NotFound();

            opportunity.CompanyId = vm.CompanyId;
            opportunity.ContactId = vm.ContactId;
            opportunity.Name = vm.Name;
            opportunity.StageId = vm.StageId;
            opportunity.CloseDate = vm.CloseDate;
            opportunity.AssignedTo = vm.AssignedTo;

            _context.OpportunityProducts.RemoveRange(opportunity.Products);

            foreach (var p in vm.Products)
            {
                _context.OpportunityProducts.Add(new OpportunityProduct
                {
                    OpportunityId = opportunity.Id,
                    ProductId = p.ProductId,
                    Quantity = p.Quantity,
                    Price = p.Price
                });
            }

            await _context.SaveChangesAsync();

            opportunity.Amount = await _context.OpportunityProducts
                .Where(x => x.OpportunityId == opportunity.Id)
                .SumAsync(x => x.Quantity * x.Price);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // VIEWMODEL BUILDER (FIXED CORE)
        // =========================
        private OpportunityFormVM BuildFormVM(OpportunityFormVM vm)
        {
            vm.Companies = _context.Companies
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList();

            vm.Contacts = _context.Contacts
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.FullName
                }).ToList();

            vm.Stages = _context.OpportunityStages
                .Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name
                }).ToList();

            vm.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = u.FullName
                }).ToList();

            vm.ProductsData = _context.Products
                .Where(p => p.IsActive)
                .ToList();

            return vm;
        }
    }
}