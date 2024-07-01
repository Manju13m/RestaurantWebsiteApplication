using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsiteApplication.Data;
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
        public IActionResult CustomerDashboard()
        {
            return View();
        }

        public async Task<IActionResult> CustomeDashboard()
        {
            // Get the customer's upcoming bookings from the database
            var customerId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
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

            // Pass the upcoming bookings to the view
            return View(upcomingBookings);
        }
    }
}
