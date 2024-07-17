using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsiteApplication.Models.ViewModels
{
    public class AddBookRequest
    {
        public string CustomerName { get; set; }

        [Display(Name = "Booking Date")]
        [Required(ErrorMessage = "Booking Date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BookingDate { get; set; }

        [Display(Name = "From Time")]
        [Required(ErrorMessage = "From Time is required")]

        public TimeSpan FromTime { get; set; }

        [Display(Name = "To Time")]
        [Required(ErrorMessage = "To Time is required")]

        public TimeSpan ToTime { get; set; }

        [Display(Name = "Table Number")]
        [Required(ErrorMessage = "Table Number is required")]
        public int TableNumber { get; set; }
        public Guid BookingId { get; set; }
        public BookingStatus Status { get; set; }
        public string UserId { get; set; } // Added UserId property
    }
}
