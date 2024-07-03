using Microsoft.AspNetCore.Mvc;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.Models.ViewModels;
using RestaurantWebsiteApplication.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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
       

        [HttpPost]
        public async Task<IActionResult> Cancel(Guid BookingId)
        {
            var booking = await restrauntDbContext.Bookingdata.FindAsync(BookingId);

            if (booking == null)
            {
                return NotFound();
            }

            // Check if the booking is cancellable (24 hours before BookingDate)
            if (booking.BookingDate > DateTime.Now.AddHours(24))
            {
                booking.Status = BookingStatus.Cancelled;

                try
                {
                    restrauntDbContext.Update(booking);
                    await restrauntDbContext.SaveChangesAsync();
                    return RedirectToAction("CustomerDashboard", "Customer"); 
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError("", "Error occurred while cancelling booking.");
                    return View("Error"); // Handle the error appropriately
                }
            }
            else
            {
                // Handle error or inform user that booking cannot be canceled
                ModelState.AddModelError("", "Booking cannot be cancelled less than 24 hours before the booking time.");
                return RedirectToAction("CustomerDashboard", ""); 
            }
        }


    }
}

        