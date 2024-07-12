using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsiteApplication.Models
{
    public class CheckIn
    {
        public int CheckInId { get; set; }
        public string CustomerId { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CheckInDate { get; set; }
        public TimeSpan CheckInTime { get; set; }
    }
}
