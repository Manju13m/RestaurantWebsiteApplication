using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsiteApplication.Models
{

    public enum BookingStatus
    {
        Booked ,
        Cancelled 
        
    }
    public class Booking
    {
        [Key]
        public Guid BookingId { get; set; } // Primary key for Booking table

        [ForeignKey("Customer")]
        public Guid UserId { get; set; } // Foreign key

        [Required(ErrorMessage = "Customer Name is required")]
        [StringLength(40, ErrorMessage = "Customer Name cannot exceed 40 characters")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "Booking Date and Time is required")]
        [Display(Name = "Booking Date")]
        [DataType(DataType.Date)] // Specify the type as Date only
        public DateTime BookingDate { get; set; } // Date and time of the booking

        public TimeSpan FromTime { get; set; }
        public TimeSpan ToTime { get; set; }

        [Required(ErrorMessage = "Table Number is required")]
        public int TableNumber { get; set; } // Table number booked
        public BookingStatus Status { get; set; } // Add this property for tracking booking status


        public Customer Customer { get; set; } // Navigation property to Customer table
    }
}
