using AIPlacement.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AIPlacement.Application.Authentication;

namespace AIPlacement.MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole(RoleNames.Admin)) return RedirectToAction("Dashboard", "Admin");
                if (User.IsInRole(RoleNames.Company)) return RedirectToAction("Index", "CompanyDashboard");
                if (User.IsInRole(RoleNames.Student)) return RedirectToAction("Index", "StudentDashboard");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
