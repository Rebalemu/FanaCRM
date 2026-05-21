using FanaCRM.Models;
using FanaCRM.Services.Interfaces;
using FanaCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FanaCRM.Controllers
{
    [Authorize]
    public class ActivityController : Controller
    {
        private readonly IActivityService _service;
        private readonly UserManager<Users> _userManager;

        public ActivityController(
            IActivityService service,
            UserManager<Users> userManager)
        {
            _service = service;
            _userManager = userManager;
        }

        // =====================================================
        // INDEX
        // =====================================================

        public async Task<IActionResult> Index()
        {
            var activities = await _service.GetAllAsync();

            return View(activities);
        }

        // =====================================================
        // CREATE GET
        // =====================================================

        public async Task<IActionResult> Create()
        {
            return View(await _service.GetCreateVMAsync());
        }

        // =====================================================
        // CREATE POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivityFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                var reloadVm = await _service.GetCreateVMAsync();

                vm.Types = reloadVm.Types;
                vm.Statuses = reloadVm.Statuses;
                vm.Companies = reloadVm.Companies;
                vm.Contacts = reloadVm.Contacts;
                vm.Leads = reloadVm.Leads;
                vm.Opportunities = reloadVm.Opportunities;
                vm.Users = reloadVm.Users;

                return View(vm);
            }

            var userId = _userManager.GetUserId(User);

            await _service.CreateAsync(vm, userId);

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // DETAILS
        // =====================================================

        public async Task<IActionResult> Details(int id)
        {
            var activity = await _service.GetDetailsAsync(id);

            if (activity == null)
                return NotFound();

            return View(activity);
        }

        // =====================================================
        // EDIT GET
        // =====================================================

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await _service.GetEditVMAsync(id);

            if (vm == null)
                return NotFound();

            return View(vm);
        }

        // =====================================================
        // EDIT POST
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ActivityFormVM vm)
        {
            if (!ModelState.IsValid)
            {
                var reloadVm = await _service.GetCreateVMAsync();

                vm.Types = reloadVm.Types;
                vm.Statuses = reloadVm.Statuses;
                vm.Companies = reloadVm.Companies;
                vm.Contacts = reloadVm.Contacts;
                vm.Leads = reloadVm.Leads;
                vm.Opportunities = reloadVm.Opportunities;
                vm.Users = reloadVm.Users;

                return View(vm);
            }

            var userId = _userManager.GetUserId(User);

            await _service.UpdateAsync(vm, userId);

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // COMPLETE
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(int id)
        {
            var userId = _userManager.GetUserId(User);

            await _service.CompleteAsync(id, userId);

            return RedirectToAction(nameof(Index));
        }

        // =====================================================
        // CANCEL
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = _userManager.GetUserId(User);
            await _service.CancelAsync(id, userId);

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Dashboard()
        {
            var userId = _userManager.GetUserId(User);

            if (string.IsNullOrEmpty(userId))
            {
                return Content("User ID is NULL");
            }

            var vm = await _service.GetDashboardAsync(userId);

            return View(vm);
        }
    }
}