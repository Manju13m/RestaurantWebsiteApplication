﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.Models;
using RestaurantWebsiteApplication.Models.ViewModels;
using System.Security.Claims;

namespace RestaurantWebsiteApplication.Controllers
{
    public class CustomerController : Controller
    {

        private readonly RestaurantDbContext restrauntDbContext;
        public CustomerController(RestaurantDbContext restrauntDbContext)
        {
            this.restrauntDbContext = restrauntDbContext;

        }

        public async Task<IActionResult> CustomerDashboard()
        {

            // Ensure customer is authenticated
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Log", "Login"); // Redirect to login page if not authenticated
            }

            // Get the customer's ID from claims
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null || !Guid.TryParse(claim.Value, out var customerId))
            {
                return RedirectToAction("Log", "Login");
            }

            // Fetch the customer's name from your database or any source
            var customer = await restrauntDbContext.Customerdata
                .FirstOrDefaultAsync(c => c.UserId ==customerId);

            // Check if customer exists (optional)
            if (customer == null)
            {
                return RedirectToAction("Error", "Home"); // Handle scenario where customer doesn't exist
            }


            // Prepare view model or view bag to pass customer's name to the view
            ViewBag.CustomerName = $"{customer.FirstName} {customer.LastName}"; // Example: Combine first and last name


            // Get the customer's upcoming bookings from the database
            var upcomingBookings = await restrauntDbContext.Bookingdata
                .Where(b => b.UserId == customerId && b.BookingDate >= DateTime.Now && b.BookingDate <= DateTime.Now.AddDays(3))
                .Select(b => new AddBookRequest
                {
                    UserId = b.UserId,
                    CustomerName = b.CustomerName,
                    BookingDate = b.BookingDate,
                    FromTime = b.FromTime,
                    ToTime = b.ToTime,
                    TableNumber = b.TableNumber
                })
                .ToListAsync();



            return View(upcomingBookings);
        }
    }
}


