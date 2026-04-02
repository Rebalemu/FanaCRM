
using Microsoft.AspNetCore.Mvc;


namespace FanaCRM.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }


    public IActionResult Services()
    {
        return View();
    }
    public IActionResult Contact()
    {
        return View();
    }
    [HttpPost]
    public IActionResult Contact(string Name, string Email, string Message, string Location, string Latitude, string Longitude)
    {
        // You now have GPS data
        // Save to DB or process it

        TempData["Success"] = "Message sent successfully!";
        return RedirectToAction("Contact");
    }


}
