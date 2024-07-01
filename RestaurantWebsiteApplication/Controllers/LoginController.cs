using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.Models.ViewModels;
using System.Security.Claims;

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

                    // Create claims for the authenticated customer
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, customer.UserId.ToString()),
                        new Claim(ClaimTypes.Name, customer.FirstName + " " + customer.LastName),
                        new Claim(ClaimTypes.Role, "Customer")
                    };
                    // Create identity object for the customer
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Sign in the customer using cookie-based authentication
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                    // Redirect to Customer Dashboard
                    return RedirectToAction("CustomerDashboard", "Customer");
                }

                // Check if UserId belongs to an Admin
                var admin = await restrauntDbContext.Admindata
                    .FirstOrDefaultAsync(a => a.UserId == addLogRequest.UserId && a.Password == addLogRequest.Password);

                if (admin != null)
                {
                    // Create claims for the authenticated admin
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.FirstName + " " + admin.LastName),
                new Claim(ClaimTypes.Role, "Admin")
                // Add more claims if needed, such as admin ID, etc.
            };

                    // Create identity object for the admin
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    // Sign in the admin using cookie-based authentication
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));


                    // Redirect to Admin Dashboard
                    return RedirectToAction("AdminDashboard", "Admin");
                }
                // If login fails, return to login page with error
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(addLogRequest);
        }


        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
