namespace RestaurantWebsiteApplication.Models.ViewModels
{
    public class AddBookRequest
    {
        public Guid BookingId { get; set; }
        public Guid UserId { get; set; }
        public string CustomerName { get; set; }
        public DateTime BookingDate { get; set; }
        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }
        public int TableNumber { get; set; }
        public BookingStatus Status { get; set; }
    }
}
