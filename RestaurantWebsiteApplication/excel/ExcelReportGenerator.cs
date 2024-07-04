using OfficeOpenXml;
using RestaurantWebsiteApplication.Data;
using System.Collections.Generic;
using System.Linq;


namespace RestaurantWebsiteApplication.excel
{
    public class ExcelReportGenerator : IExcelReportGenerator
    {
        private readonly RestaurantDbContext _context;

        public ExcelReportGenerator(RestaurantDbContext context)
        {
            _context = context;
        }

        public byte[] GenerateBookingsReport(DateTime startDate, DateTime endDate)
        {
            var bookings = _context.Bookingdata
        .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate)
        .Select(b => new {
            b.BookingId,
            b.CustomerName,
            b.BookingDate,
            b.TableNumber,
            b.Status
        })
        .ToList();

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Bookings Report");
                worksheet.Cells.LoadFromCollection(bookings, true);

                worksheet.Column(3).Style.Numberformat.Format = "mm/dd/yyyy"; // Format date column
                worksheet.Column(1).Width = 20; // Adjust column width
                return package.GetAsByteArray();
            }
        }
    }
}


