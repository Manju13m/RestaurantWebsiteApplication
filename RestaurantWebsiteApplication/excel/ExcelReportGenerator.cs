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

                // Format date column
                worksheet.Column(3).Style.Numberformat.Format = "dd/MM/yyyy";

                // Set column widths
                worksheet.Column(1).Width = 36; // Adjust width for BookingId (GUID)
                worksheet.Column(2).Width = 25; // Adjust width for CustomerName
                worksheet.Column(3).Width = 15; // Adjust width for BookingDate
                worksheet.Column(4).Width = 15; // Adjust width for TableNumber
                worksheet.Column(5).Width = 15; // Adjust width for Status

                return package.GetAsByteArray();
            }
        }
    }
}


