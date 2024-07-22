using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhoneReclaim.Models;

namespace PhoneReclaim.Data.Models
{
    public class WishlistItem
    {
        public Guid Id { get; set; }
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}