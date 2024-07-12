using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsiteApplication.Models.ViewModels
{
    public class CheckOutViewModel
    {
        
        public string CustomerId { get; set; }
        [Required]
        public decimal GrossAmount { get; set; }
    }
}
