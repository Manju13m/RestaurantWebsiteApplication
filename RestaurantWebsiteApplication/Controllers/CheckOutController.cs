using Microsoft.AspNetCore.Mvc;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.email;
using RestaurantWebsiteApplication.Models;
using RestaurantWebsiteApplication.Models.ViewModels;

namespace RestaurantWebsiteApplication.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;
        private readonly IEmailService emailService;
        public CheckOutController(RestaurantDbContext restrauntDbContext, IEmailService emailService)
        {
            this.restrauntDbContext = restrauntDbContext;
            this.emailService = emailService;
        }
        public IActionResult CheckOut()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(CheckOutViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Assuming RestaurantDbContext is properly injected or initialized
                var checkOut = new CheckOut
                {
                    CustomerId = model.CustomerId,
                    GrossAmount = model.GrossAmount
                };

                restrauntDbContext.CheckOuts.Add(checkOut);
                await restrauntDbContext.SaveChangesAsync();

                string email = GetCustomerEmailById(model.CustomerId);
                if (string.IsNullOrEmpty(email))
                {
                    ModelState.AddModelError("", "Customer email address not found.");
                    return View(model);
                }

                string subject = "Your Checkout Details";
                string message = $"Dear Customer,\n\nThank you for dining with us! \n\nYour gross amount for the recent visit is ₹{model.GrossAmount:N0}. \n\nWe appreciate your patronage. \n\nBest regards, \nTrupthi Restaurant";

                try
                {
                    await emailService.SendEmailAsync(email, subject, message);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error sending email: {ex.Message}");
                    return View(model);
                }

                return RedirectToAction("CheckOutConfirmation"); // Redirect to a success page or action
            }

            // If ModelState is not valid, return to the view with validation errors
            return View(model);
        }

        public IActionResult CheckOutConfirmation()
        {
            return View();
        }

        private string GetCustomerEmailById(string customerId)
        {
            // Implement logic to fetch the customer's email address using their ID
            // Assuming customerId is int and UserId is Guid
            var customer = restrauntDbContext.Customerdata.FirstOrDefault(c => c.UserId == customerId);
            return customer?.Email;
        }
    }
    
}
