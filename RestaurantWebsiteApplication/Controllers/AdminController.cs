using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsiteApplication.Data;
using System.Security.Claims;

namespace RestaurantWebsiteApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;

        public AdminController(RestaurantDbContext restrauntDbContext)
        {
            this.restrauntDbContext = restrauntDbContext;
        }
        public async Task<IActionResult> AdminDashboardAsync()
        {
            // Assuming you have a way to get the logged-in admin's name
            // For example, if you are using claims-based authentication
            var adminName = User.FindFirstValue(ClaimTypes.Name);

            ViewData["AdminName"] = adminName;
            
            

            

            return View();
        }
    }
}

