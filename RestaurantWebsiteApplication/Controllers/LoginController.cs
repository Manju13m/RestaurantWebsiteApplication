using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.Models.ViewModels;

namespace RestaurantWebsiteApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;

        public LoginController(RestaurantDbContext restrauntDbContext)
        {
            this.restrauntDbContext = restrauntDbContext;
        }
        [HttpGet]
        public IActionResult Log()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Log(AddLogRequest addLogRequest)
        {
            if (ModelState.IsValid)
            {
                // Check if UserId belongs to a Customer
                var customer = await restrauntDbContext.Customerdata
                    .FirstOrDefaultAsync(c => c.UserId == addLogRequest.UserId && c.Password == addLogRequest.Password);

                if (customer != null)
                {
                    // Redirect to Customer Dashboard
                    return RedirectToAction("CustomerDashboard", "Customer");
                }

                // Check if UserId belongs to an Admin
                var admin = await restrauntDbContext.Admindata
                    .FirstOrDefaultAsync(a => a.UserId == addLogRequest.UserId && a.Password == addLogRequest.Password);

                if (admin != null)
                {
                    // Redirect to Admin Dashboard
                    return RedirectToAction("AdminDashboard", "Admin");
                }
                // If login fails, return to login page with error
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(addLogRequest);
        }
    }
}
