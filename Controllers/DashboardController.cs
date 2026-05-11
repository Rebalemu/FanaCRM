
using Microsoft.AspNetCore.Mvc;


namespace FanaCRM.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "Admin");
            }

            if (User.IsInRole("Sales"))
            {
                return RedirectToAction("Index", "Sales");
            }

            if (User.IsInRole("Support"))
            {
                return RedirectToAction("Index", "Support");
            }

            return Unauthorized();
        }
    }
}