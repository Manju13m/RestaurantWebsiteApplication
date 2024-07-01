using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace RestaurantWebsiteApplication.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminDashboard()
        {
            // Assuming you have a way to get the logged-in admin's name
            // For example, if you are using claims-based authentication
            var adminName = User.FindFirstValue(ClaimTypes.Name);

            ViewData["AdminName"] = adminName;
            return View();
        }
    }
}
