using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.Models;
using RestaurantWebsiteApplication.Models.ViewModels;
using System.Diagnostics;
using System.Reflection.Metadata;
using System;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Threading.Channels;
using RestaurantWebsiteApplication.excel;
using Microsoft.AspNetCore.Http.HttpResults;
using RestaurantWebsiteApplication.email;


namespace RestaurantWebsiteApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;
        private readonly IExcelReportGenerator _excelReportGenerator;
        private readonly IEmailService emailService;

        public AdminController(RestaurantDbContext restrauntDbContext, IExcelReportGenerator excelReportGenerator, IEmailService emailService)
        {
            this.restrauntDbContext = restrauntDbContext;
            _excelReportGenerator = excelReportGenerator;
            this.emailService = emailService;

        }

        public async Task<IActionResult> AdminDashboard()
        {

            var adminName = User.FindFirstValue(ClaimTypes.Name);

            ViewData["AdminName"] = adminName;

            // Get total customers registered in the last 7 days
            var totalCustomersLast7Days = await restrauntDbContext.Customerdata
                .Where(c => c.CreatedAt >= DateTime.UtcNow.AddDays(-7))
                .CountAsync();

            // Get total bookings done for the next 3 days
            var totalBookingsNext3Days = await restrauntDbContext.Bookingdata
                .Where(b => b.BookingDate >= DateTime.UtcNow && b.BookingDate <= DateTime.UtcNow.AddDays(3))
                .CountAsync();

            // Get total cancellations in the last 3 days
            var totalCancellationsLast3Days = await restrauntDbContext.Bookingdata
                .Where(b => b.Status == BookingStatus.Cancelled && b.BookingDate >= DateTime.UtcNow.AddDays(-3))
                .CountAsync();

            // Get upcoming bookings details
            DateTime today = DateTime.Today;
            DateTime threeDaysLater = today.AddDays(3);

            var upcomingBookings = await restrauntDbContext.Bookingdata
                .Where(b => b.BookingDate >= today && b.BookingDate <= threeDaysLater)
                .Select(b => new AddBookRequest
                {
                    UserId = b.UserId,
                    BookingId = b.BookingId,
                    FromTime = b.FromTime,
                    ToTime = b.ToTime,
                    BookingDate = b.BookingDate,
                    TableNumber = b.TableNumber
                })
                .ToListAsync();

            ViewBag.TotalCustomersLast7Days = totalCustomersLast7Days;
            ViewBag.TotalBookingsNext3Days = totalBookingsNext3Days;
            ViewBag.TotalCancellationsLast3Days = totalCancellationsLast3Days;

            return View(upcomingBookings);
        }
        public IActionResult DownloadBookingsReport(DateTime startDate, DateTime endDate)
        {
            try
            {
                // Generate Excel report using injected service
                var report = _excelReportGenerator.GenerateBookingsReport(startDate, endDate);

                // Return the report as a downloadable Excel file
                return File(report, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "BookingsReport.xlsx");
            }
            catch (Exception ex)
            {
                // Handle exceptions (log or display an error message)
                ViewBag.ErrorMessage = $"Error generating bookings report: {ex.Message}";
                return View("Error"); // Create an Error.cshtml view to display the error message
            }
        }

        // GET: Admin/CheckIn
        public IActionResult CheckIn()
        {
            return View();
        }

        // POST: Admin/CheckIn
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CheckIn(Guid customerId, DateTime checkInDateTime)
        {
            // Find the customer by UserId (assuming it's Guid)
            var customer = restrauntDbContext.Customerdata.FirstOrDefault(c => c.UserId == customerId);

            if (customer != null)
            {
                // Update the customer's check-in date/time
                //customer.CheckInDateTime = checkInDateTime;

                // Save changes to the database
                restrauntDbContext.SaveChanges();

                // Optionally, send an email notification to the customer
                // Call your email service method here if needed

                // Redirect back to the admin dashboard or return a success message
                return RedirectToAction("AdminDashboard");
            }
            else
            {
                // Handle case where customer with given customerId was not found
                // Perhaps return an error message or redirect with a notification
                return RedirectToAction("AdminDashboard"); // Or another action as needed
            }
        }

        // GET: Admin/CheckOut
        public IActionResult CheckOut()
        {
            return View();
        }

        // POST: Admin/CheckOut
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<IActionResult> CheckOut(string customerId, decimal grossAmount)
        {
            var checkOut = new CheckOut
            {
                CustomerId = customerId,
              
                GrossAmount = grossAmount
            };

            restrauntDbContext.CheckOuts.Add(checkOut);
            await restrauntDbContext.SaveChangesAsync();

            // Send email to the customer
            string email = GetCustomerEmailById(customerId);
            string subject = "Your Checkout Details";
            string message = $"Dear Customer, \n\nYour gross amount for the recent visit is {grossAmount:C}. \n\nThank you for dining with us! \n\nBest regards, \nTrupthi Restaurant";

            await emailService.SendEmailAsync(email, subject, message);

            return RedirectToAction("AdminDashboard");
        }

        private string GetCustomerEmailById(string customerId)
        {
            // Implement your logic to fetch the customer's email address using their ID
            // Assuming customerId is int and UserId is Guid
            var customer = restrauntDbContext.Customerdata.FirstOrDefault(c => c.UserId == Guid.Parse(customerId.ToString()));
            return customer?.Email;
        }


    }
}