using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsiteApplication.Models.ViewModels
{
    public class AddLogRequest
    {
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
