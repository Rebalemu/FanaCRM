using Microsoft.AspNetCore.Mvc;
using FanaCRM.Services;
using FanaCRM.ViewModels;

namespace FanaCRM.Controllers
{
    public class SalesController : Controller
    {
        private readonly ISalesDashboardService _service;

        public SalesController(ISalesDashboardService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string filter = "month")
        {
            var model = await _service.GetDashboardDataAsync(filter);
            return View(model);
        }
    }
}