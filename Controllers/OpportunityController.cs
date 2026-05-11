using FanaCRM.Services.Interfaces;
using FanaCRM.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace FanaCRM.Controllers
{
    public class OpportunityController : Controller
    {
        private readonly IOpportunityService _service;

        public OpportunityController(IOpportunityService service)
        {
            _service = service;
        }

        // =========================
        // INDEX
        // =========================
        public async Task<IActionResult> Index(string? search, int? stageId)
        {
            var data = await _service.GetAllAsync(search, stageId);

            ViewBag.Stages = await _service.GetStagesDropdownAsync();

            return View(data);
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int id)
        {
            var vm = await _service.GetDetailsAsync(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        // =========================
        // CREATE GET
        // =========================
        public async Task<IActionResult> Create()
        {
            var vm = new OpportunityFormVM
            {
                Products = new List<OpportunityProductVM>
        {
            new OpportunityProductVM
            {
                Quantity = 1
            }
        }
            };

            vm = await _service.BuildFormVMAsync(vm);

            return View(vm);
        }

        // =========================
        // CREATE POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OpportunityFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                vm = await _service.BuildFormVMAsync(vm);

                return View(vm);
            }
            if (vm.Products == null || !vm.Products.Any())
            {
                vm.Products = new List<OpportunityProductVM>
                   {
                      new OpportunityProductVM()
                   };
            }

            var result = await _service.CreateAsync(vm);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error);

                vm = await _service.BuildFormVMAsync(vm);

                return View(vm);
            }

            TempData["Success"] = "Opportunity created successfully";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT GET
        // =========================
        public async Task<IActionResult> Edit(int id)
        {
            var details = await _service.GetDetailsAsync(id);

            if (details == null)
                return NotFound();

            var vm = new OpportunityFormVM
            {
                Id = details.Id,
                Name = details.Name,

                CompanyId = details.CompanyId,

                ContactId = details.ContactId,

                StageId = details.StageId,

                CloseDate = details.CloseDate,

                AssignedTo = details.AssignedToId,

                LossReason = details.LossReason,

                Products = details.Products ?? new List<OpportunityProductVM>()
            };

            vm = await _service.BuildFormVMAsync(vm);

            return View(vm);
        }

        // =========================
        // EDIT POST
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OpportunityFormVM vm)
        {
            if (id != vm.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                vm = await _service.BuildFormVMAsync(vm);

                return View(vm);
            }

            var result = await _service.UpdateAsync(id, vm);

            if (!result.Success)
            {
                ModelState.AddModelError("", result.Error);

                vm = await _service.BuildFormVMAsync(vm);

                return View(vm);
            }

            TempData["Success"] = "Opportunity updated successfully";

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // DELETE
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);

            TempData["Success"] = "Opportunity deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}