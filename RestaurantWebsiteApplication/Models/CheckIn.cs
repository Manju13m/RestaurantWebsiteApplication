namespace RestaurantWebsiteApplication.Models
{
    public class CheckIn
    {
        public int CheckInId { get; set; }
        public string CustomerId { get; set; }
        public DateTime CheckInDate { get; set; }
        public TimeSpan CheckInTime { get; set; }
    }
}
