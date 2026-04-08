
using FanaCRM.Models;
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

        public AdminController(UserManager<Users> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            return View();
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

        // GET EDIT
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

        // POST EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(EditRoleVM model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return NotFound();

            model.SelectedRoles = model.SelectedRoles ?? new List<string>();

            // ❌ No roles selected → return back to view
            if (!model.SelectedRoles.Any())
            {
                model.SelectedRoles.Add("User"); // ✅ default role
                TempData["Info"] = "Default Role Assigned ✅";

                // reload roles (VERY IMPORTANT)
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

            return RedirectToAction("UserList");
        }
        // POST Delete user

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            // Optional: prevent deleting yourself
            var currentUserId = _userManager.GetUserId(User);
            if (user.Id == currentUserId)
            {
                TempData["Error"] = "You cannot delete your own account!";
                return RedirectToAction("UserList");
            }

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["Success"] = "User deleted successfully ✅";
            }
            else
            {
                TempData["Error"] = "Error deleting user ❌";
            }

            return RedirectToAction("UserList");
        }
    }
}