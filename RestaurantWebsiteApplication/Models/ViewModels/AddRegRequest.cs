using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsiteApplication.Models.ViewModels
{
    public class AddRegRequest
    {
        [Required]
        public string UserId { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        [MaxLength(20)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        [MaxLength(20)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(50)]
        public string Address { get; set; }

        
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"\d{10}", ErrorMessage = "Phone Number must be 10 digits")]
        public string PhoneNo { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public string Role { get; set; }


        
    }
}
