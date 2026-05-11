using FanaCRM.Models;
using FanaCRM.Services;
using FanaCRM.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FanaCRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IDashboardService _dashboardService;

        public AdminController(
            UserManager<Users> userManager,
            RoleManager<IdentityRole> roleManager,
            IDashboardService dashboardService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dashboardService = dashboardService;
        }

        public async Task<IActionResult> Index(string filter = "month")
        {
            var vm = await _dashboardService.GetDashboardDataAsync(filter);
            return View(vm);
        }

        public async Task<IActionResult> UserList()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserVM>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserVM
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(model);
        }

        public async Task<IActionResult> EditRole(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var allRoles = _roleManager.Roles.ToList();
            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditRoleVM
            {
                UserId = user.Id,
                Email = user.Email,
                SelectedRoles = userRoles.ToList(),
                Roles = allRoles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditRoleVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return NotFound();

            model.SelectedRoles ??= new List<string>();

            if (!model.SelectedRoles.Any())
            {
                model.SelectedRoles.Add("User");
                TempData["Info"] = "Default Role Assigned";

                model.Roles = _roleManager.Roles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList();

                return View(model);
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            var rolesToRemove = currentRoles.Except(model.SelectedRoles);
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);

            var rolesToAdd = model.SelectedRoles.Except(currentRoles);
            await _userManager.AddToRolesAsync(user, rolesToAdd);

            return RedirectToAction(nameof(UserList));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "You cannot delete your own account!";
                return RedirectToAction(nameof(UserList));
            }

            var result = await _userManager.DeleteAsync(user);

            TempData[result.Succeeded ? "Success" : "Error"] =
                result.Succeeded ? "User deleted successfully" : "Error deleting user";

            return RedirectToAction(nameof(UserList));
        }
    }
}