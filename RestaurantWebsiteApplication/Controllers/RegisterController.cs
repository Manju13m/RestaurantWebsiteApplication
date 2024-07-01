using Microsoft.AspNetCore.Mvc;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.Models;
using RestaurantWebsiteApplication.Models.ViewModels;

namespace RestaurantWebsiteApplication.Controllers
{
    public class RegisterController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;

        public RegisterController(RestaurantDbContext restrauntDbContext)
        {
            this.restrauntDbContext = restrauntDbContext;
        }
        [HttpGet]
        public IActionResult Reg()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Reg")]
        public async Task<IActionResult> Reg(AddRegRequest addRegRequest)
        {
            if (!ModelState.IsValid)
            {
                return View(addRegRequest);
            }
            if (addRegRequest.Role == "Customer")
            {
                var customer = new Customer
                {
                    //mapping AddRegRequest to Customer model
                    FirstName = addRegRequest.FirstName,
                    LastName = addRegRequest.LastName,
                    Address = addRegRequest.Address,

                    Password = addRegRequest.Password,
                    PhoneNo = addRegRequest.PhoneNo,
                    Email = addRegRequest.Email,
                };
                restrauntDbContext.Customerdata.Add(customer);
                await restrauntDbContext.SaveChangesAsync();

            }

            else if (addRegRequest.Role == "Admin")
            {
                var admin = new Admin
                {
                    FirstName = addRegRequest.FirstName,
                    LastName = addRegRequest.LastName,
                    Address = addRegRequest.Address,

                    Password = addRegRequest.Password,
                    PhoneNo = addRegRequest.PhoneNo,
                    Email = addRegRequest.Email,
                };
                restrauntDbContext.Admindata.Add(admin);
                await restrauntDbContext.SaveChangesAsync();

            }


            // Redirect with success query parameter
            return RedirectToAction("Reg", new { success = true });

        }
    }
}
