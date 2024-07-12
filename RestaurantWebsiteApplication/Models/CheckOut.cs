﻿using System.ComponentModel.DataAnnotations;

namespace RestaurantWebsiteApplication.Models
{
    public class CheckOut
    {
        public int CheckOutId { get; set; }
        [Required]
        public string CustomerId { get; set; }
        [Required]
        public decimal GrossAmount { get; set; }
    }
}

