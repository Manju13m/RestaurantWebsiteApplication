using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.Models.ViewModels;
using System.Security.Claims;
using RestaurantWebsiteApplication.Password;

namespace RestaurantWebsiteApplication.Controllers
{
    public class LoginController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;
        private readonly PasswordService _passwordService;

        public LoginController(RestaurantDbContext restrauntDbContext, PasswordService passwordService)
        {
            this.restrauntDbContext = restrauntDbContext;
            this._passwordService = passwordService;
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
                    .FirstOrDefaultAsync(c => c.UserId == addLogRequest.UserId);

                if (customer != null && _passwordService.VerifyPassword(addLogRequest.Password, customer.PasswordHash, customer.PasswordSalt))

                {

                    // Create claims for the authenticated customer
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, customer.UserId.ToString()),
                        new Claim(ClaimTypes.Name, customer.FirstName + " " + customer.LastName),
                        new Claim(ClaimTypes.Email, customer.Email),
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
                     .FirstOrDefaultAsync(a => a.UserId == addLogRequest.UserId);

                if (admin != null && _passwordService.VerifyPassword(addLogRequest.Password, admin.PasswordHash, admin.PasswordSalt))
                {

                    // Create claims for the authenticated admin
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.FirstName + " " + admin.LastName),
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Role, "Admin")
               
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
