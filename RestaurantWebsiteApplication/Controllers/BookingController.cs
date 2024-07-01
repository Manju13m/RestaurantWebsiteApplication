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
        public IActionResult Book()
        {
            return View();
        }

        public async Task<IActionResult> Book(AddBookRequest addBookRequest)
        {
            if (!User.Identity.IsAuthenticated)
            {
                ModelState.AddModelError("", "User is not authenticated.");
                return View(addBookRequest);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                ModelState.AddModelError("", "User ID claim is not available.");
                return View(addBookRequest);
            }



            if (ModelState.IsValid)
            {
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
