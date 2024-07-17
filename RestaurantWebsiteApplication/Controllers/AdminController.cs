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
using Microsoft.SqlServer.Server;
using System.Globalization;


namespace RestaurantWebsiteApplication.Controllers
{
    public class AdminController : Controller
    {
        private readonly RestaurantDbContext restrauntDbContext;
        private readonly IExcelReportGenerator _excelReportGenerator;


        public AdminController(RestaurantDbContext restrauntDbContext, IExcelReportGenerator excelReportGenerator)
        {
            this.restrauntDbContext = restrauntDbContext;
            _excelReportGenerator = excelReportGenerator;

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
                .Where(b => b.BookingDate >= today && b.BookingDate <= threeDaysLater && b.Status == BookingStatus.Booked)
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
                // Format the dates to dd/MM/yyyy format for display or further processing
                string formattedStartDate = startDate.ToString("dd/MM/yyyy");
                string formattedEndDate = endDate.ToString("dd/MM/yyyy");


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
    }
}