namespace RestaurantWebsiteApplication.excel
{
    public interface IExcelReportGenerator
    {
        byte[] GenerateBookingsReport(DateTime startDate, DateTime endDate);
    }
}
