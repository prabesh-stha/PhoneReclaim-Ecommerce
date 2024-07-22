using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PhoneReclaim.Models;

namespace PhoneReclaim.Data.Models
{
    public class AppUser: IdentityUser
    {
        public string? Name { get; set;}
        
        public int? Phone { get; set;}

        public string? ImageUrl { get; set; }

        public ICollection<Product>? Products { get; set; }
         public ICollection<WishlistItem>? WishlistItems { get; set; } 
        
    }
}