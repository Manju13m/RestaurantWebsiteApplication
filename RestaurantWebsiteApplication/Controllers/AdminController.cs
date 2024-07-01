using Microsoft.AspNetCore.Mvc;

namespace RestaurantWebsiteApplication.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult AdminDashboard()
        {
            return View();
        }
    }
}
