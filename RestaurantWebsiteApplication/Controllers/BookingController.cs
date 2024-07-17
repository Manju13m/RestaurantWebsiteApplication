﻿using Microsoft.AspNetCore.Mvc;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.Models.ViewModels;
using RestaurantWebsiteApplication.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using RestaurantWebsiteApplication.email;
using Microsoft.AspNetCore.Authorization;

namespace RestaurantWebsiteApplication.Controllers
{
    public class BookingController : Controller
    {

        private readonly RestaurantDbContext restrauntDbContext;
        private readonly IEmailService emailService;
        private readonly ILogger<BookingController> logger;

        public BookingController(RestaurantDbContext restrauntDbContext, IEmailService emailService, ILogger<BookingController> logger)
        {
            this.restrauntDbContext = restrauntDbContext;
            this.emailService = emailService;
            this.logger = logger;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Book()
        {

            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get UserId from claims

            AddBookRequest bookingViewModel = new AddBookRequest
            {
                UserId = userId
            };

            return View(bookingViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Book(AddBookRequest addBookRequest)
        {
            
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
                    logger.LogInformation("Booking conflict detected for table {TableNumber} on {BookingDate} from {FromTime} to {ToTime}",
                        addBookRequest.TableNumber, addBookRequest.BookingDate, addBookRequest.FromTime, addBookRequest.ToTime);

                    ModelState.AddModelError("", "The table is not available for the selected time slot.");

                    // Set ViewBag with conflict message
                    ViewBag.ConflictMessage = "The table is not available for the selected time slot.";
                    return View(addBookRequest);
                }

                Booking booking = new Booking
                {
                    BookingId = Guid.NewGuid(),
                    UserId = addBookRequest.UserId,

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

                    // Retrieve admin emails from the database
                    var adminEmails = await restrauntDbContext.Admindata
                        .Select(a => a.Email)
                        .ToListAsync();

                    // Send booking notification email to each admin
                    foreach (var adminEmail in adminEmails)
                    {
                        string adminSubject = "New Booking at Trupthi Restaurant";
                        string adminMessage = $"Dear Admin,\n\nA new booking has been made at Trupthi Restaurant:\n\nCustomer ID: {booking.UserId}\nCustomer Name: {addBookRequest.CustomerName}\nBooking Date: {addBookRequest.BookingDate.ToShortDateString()}\nTime Slot: {addBookRequest.FromTime} to {addBookRequest.ToTime}\nTable Number: {addBookRequest.TableNumber}\n\nPlease review the details and manage accordingly.\n\nBest regards,\nTrupthi Restaurant Team";

                        await emailService.SendEmailAsync(adminEmail, adminSubject, adminMessage);
                    }



                    // Send booking confirmation email to customer
                    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                   if (userEmail != null)
                    {
                        string subject = "Booking Confirmation at Trupthi Restaurant";
                        string message = $"Dear {addBookRequest.CustomerName},\n\nWe are pleased to confirm your booking at Trupthi Restaurant.\n\nHere are the details of your reservation:\nBooking Date: {addBookRequest.BookingDate.ToShortDateString()}\nTime Slot: {addBookRequest.FromTime} to {addBookRequest.ToTime}\nTable Number: {addBookRequest.TableNumber}.\n\nThank you for choosing Trupthi Restaurant. We look forward to serving you.\n\nBest regards,\nTrupthi Restaurant Team";
                        await emailService.SendEmailAsync(userEmail, subject, message);
                    }

                    

                    return RedirectToAction("CustomerDashboard", "Customer");
                }

                catch (Exception ex)
                {
                    // Log the exception
                    // Console.WriteLine(ex.Message);
                    logger.LogError(ex, "Error occurred while saving booking.");
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

                    // Retrieve admin emails from the database
                    var adminEmails = await restrauntDbContext.Admindata
                        .Select(a => a.Email)
                        .ToListAsync();

                    // Send cancellation email to each admin
                    foreach (var adminEmail in adminEmails)
                    {
                        string adminSubject = "Booking Cancellation at Trupthi Restaurant";
                        string adminMessage = $"Dear Admin,\n\nA booking has been canceled at Trupthi Restaurant:\n\n Customer Name: {booking.CustomerName}\nBooking Date: {booking.BookingDate.ToShortDateString()}\nTime Slot: {booking.FromTime} to {booking.ToTime}\nTable Number: {booking.TableNumber}\n\nPlease review the details and manage accordingly.\n\nBest regards,\nTrupthi Restaurant Team";

                        await emailService.SendEmailAsync(adminEmail, adminSubject, adminMessage);
                    }

                    // Send cancellation email to customer
                    var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
                    if (userEmail != null)
                    {
                        string subject = "Booking Cancellation at Trupthi Restaurant";
                        string message = $"Dear {booking.CustomerName},\n\nWe have received your request to cancel your booking at Trupthi Restaurant on {booking.BookingDate.ToShortDateString()} from {booking.FromTime} to {booking.ToTime} for table number {booking.TableNumber}.\n\nYour booking has been successfully canceled.\n\nThank you for choosing Trupthi Restaurant. We hope to welcome you again soon!\n\nBest regards,\nTrupthi Restaurant Team";
                        await emailService.SendEmailAsync(userEmail, subject, message);
                    }

                   


                    return RedirectToAction("CustomerDashboard", "Customer"); 
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while cancelling booking.");
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

        