using Microsoft.AspNetCore.Mvc;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.Models.ViewModels;
using RestaurantWebsiteApplication.Models;
using System.Security.Claims;

namespace RestaurantWebsiteApplication.Controllers
{
    public class BookingController : Controller
    {

        private readonly RestaurantDbContext restrauntDbContext;

        public BookingController(RestaurantDbContext restrauntDbContext)
        {
            this.restrauntDbContext = restrauntDbContext;
        }
        [HttpGet]
        public IActionResult Book()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Book(AddBookRequest addBookRequest)
        {
            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "User is not authenticated.");
                return View(addBookRequest);
            }

            // Check for user ID claim
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                ModelState.AddModelError("", "User ID claim is not available.");
                return View(addBookRequest);
            }



            if (ModelState.IsValid)
            {
                // Check for booking conflicts
                bool isConflict = restrauntDbContext.Bookingdata.Any(b => b.TableNumber == addBookRequest.TableNumber &&
                                                                          b.BookingDate == addBookRequest.BookingDate &&
                                                                          ((addBookRequest.FromTime >= b.FromTime && addBookRequest.FromTime < b.ToTime) ||
                                                                           (addBookRequest.ToTime > b.FromTime && addBookRequest.ToTime <= b.ToTime) ||
                                                                           (addBookRequest.FromTime <= b.FromTime && addBookRequest.ToTime >= b.ToTime)));

                if (isConflict)
                {
                    ModelState.AddModelError("", "The table is already booked for the selected time slot.");
                    return View(addBookRequest);
                }

                var booking = new Booking
                {
                    BookingId = Guid.NewGuid(),
                    UserId = Guid.Parse(userIdClaim.Value),
                    //Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                    CustomerName = addBookRequest.CustomerName,
                    BookingDate = addBookRequest.BookingDate,
                    FromTime = addBookRequest.FromTime,
                    ToTime = addBookRequest.ToTime,
                    TableNumber = addBookRequest.TableNumber

                };

                try
                {
                    restrauntDbContext.Bookingdata.Add(booking);
                    await restrauntDbContext.SaveChangesAsync();
                    return RedirectToAction("CustomerDashboard", "Customer");
                }

                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine(ex.Message);
                    ModelState.AddModelError("", "Error occurred while saving booking.");
                    return View(addBookRequest); // Return the view with error message
                }

            }
            return View(addBookRequest);
        }

    }
}
