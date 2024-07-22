using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using PhoneReclaim.Data.Models;

namespace PhoneReclaim.Models
{
    public class Product
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Name is Required")]
        public string? Name { get; set; }
        [Required(ErrorMessage = "Brand is Required")]
        public Brand Brand { get; set; }
        [Required(ErrorMessage = "Condition in Required")]
        public Condition Condition { get; set; }
        
        [Required(ErrorMessage = "Price is required")]
        public decimal? Price { get; set; }
        public string? Description { get; set; }
        public DateTime AddedDate {get; set; } = DateTime.Now;
        [Display(Name="Add Image")]
        public string? ImagePath { get; set; }
        public string? AppUserId { get; set; }

        // Navigation property for the user who owns this product
        public AppUser? AppUser { get; set; }
        public ICollection<WishlistItem>? WishlistItems { get; set; } 
    }
}