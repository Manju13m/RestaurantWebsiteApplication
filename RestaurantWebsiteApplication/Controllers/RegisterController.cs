using Microsoft.AspNetCore.Mvc;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.email;
using RestaurantWebsiteApplication.Models;
using RestaurantWebsiteApplication.Models.ViewModels;
using RestaurantWebsiteApplication.Password;
using System.Security.Policy;

namespace RestaurantWebsiteApplication.Controllers
{
    public class RegisterController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;
        private readonly IEmailService emailService;
        private readonly PasswordService _passwordService;

        public RegisterController(RestaurantDbContext restrauntDbContext, IEmailService emailService, PasswordService passwordService)
        {
            this.restrauntDbContext = restrauntDbContext;
            this.emailService = emailService;
            this._passwordService = passwordService;
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

            // Check if the email is already registered
            bool emailExists = restrauntDbContext.Customerdata.Any(c => c.Email == addRegRequest.Email) ||
                               restrauntDbContext.Admindata.Any(a => a.Email == addRegRequest.Email);

            if (emailExists)
            {
                ModelState.AddModelError(string.Empty, "This email ID is already registered. Please login using your credentials.");
                return View(addRegRequest);
            }

            // Hash password and generate salt
            byte[] salt = _passwordService.GenerateSalt();
            byte[] hashedPassword = _passwordService.HashPassword(addRegRequest.Password, salt);


            if (addRegRequest.Role == "Customer")
            {
                var customer = new Customer
                {
                    UserId = addRegRequest.UserId,
                    FirstName = addRegRequest.FirstName,
                    LastName = addRegRequest.LastName,
                    Address = addRegRequest.Address,
                    PasswordHash = hashedPassword,
                    PhoneNo = addRegRequest.PhoneNo,
                    Email = addRegRequest.Email,
                    PasswordSalt = salt
                };
                restrauntDbContext.Customerdata.Add(customer);
                await restrauntDbContext.SaveChangesAsync();
               


                // Send email to customer with the unique ID
                string subject = "Welcome to Trupthi Restaurant!";
                string message = $"Dear {customer.FirstName},\n\nThank you for registering with Trupthi Restaurant! We are thrilled to have you as a part of our community.\n\nYour user ID is: {customer.UserId}\n\nYou can now log in to your account using this user ID and the password you created during registration. Once logged in, you can book tables, view upcoming reservations, and manage your bookings with ease.\n\nWe look forward to serving you and hope you have a delightful dining experience with us.\n\nBest regards,\nThe Trupthi Restaurant Team";
                await emailService.SendEmailAsync(customer.Email, subject, message);


               
                TempData["SuccessMessage"] = "Registration successful!";

            }

            else if (addRegRequest.Role == "Admin")
            {
                var admin = new Admin
                {
                    UserId = addRegRequest.UserId, 
                    FirstName = addRegRequest.FirstName,
                    LastName = addRegRequest.LastName,
                    Address = addRegRequest.Address,

                    PhoneNo = addRegRequest.PhoneNo,
                    Email = addRegRequest.Email,
                    PasswordHash = hashedPassword,
                    PasswordSalt = salt
                };
                restrauntDbContext.Admindata.Add(admin);
                await restrauntDbContext.SaveChangesAsync();
               

                // Send email to admin with the unique ID
                string subject = "Welcome to Trupthi Restaurant Team!";
                string message = $"Dear {admin.FirstName},\n\nThank you for registering with Trupthi Restaurant!\n\nYour admin user ID is: {admin.UserId}\n\nYou can now log in to your admin account using this user ID and the password you created during registration.As an admin, you will have access to manage bookings, view customer registrations, and oversee all activities within our restaurant's online system.\n\nWe are excited to have you on board and look forward to working with you to deliver exceptional service to our customers.\n\nBest regards,\nThe Trupthi Restaurant Team";
                await emailService.SendEmailAsync(admin.Email, subject, message);


               
                TempData["SuccessMessage"] = "Admin registration successful!";

            }

            
            // Redirect with success query parameter
            return RedirectToAction("Reg");


        }
    }
}
