using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PhoneReclaim.Data.Models;

namespace PhoneReclaim.Controllers
{
    public class WishlistController : Controller
    {
        private readonly ILogger<WishlistController> _logger;
        private readonly UserManager<AppUser> _userManager;
        private readonly PhoneReclaimDb _db;

        public WishlistController(ILogger<WishlistController> logger, UserManager<AppUser> userManager, PhoneReclaimDb db)
        {
            _logger = logger;
            _userManager = userManager;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var wishlistItems = await _db.WishlistItems
                .Include(wi => wi.Product)
                .Where(wi => wi.AppUserId == user!.Id)
                .ToListAsync();

            return View(wishlistItems);
        }

        [HttpPost]
        public async Task<IActionResult> Toggle(Guid productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var existingWishlistItem = _db.WishlistItems.FirstOrDefault(wi => wi.AppUserId == user.Id && wi.ProductId == productId);
            if (existingWishlistItem != null)
            {
                _db.WishlistItems.Remove(existingWishlistItem);
            }
            else
            {
                var wishlistItem = new WishlistItem
                {
                    AppUserId = user.Id,
                    ProductId = productId
                };
                _db.WishlistItems.Add(wishlistItem);
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("Index","Home");
        }
             public bool IsInWishlist(Guid productId, AppUser user)
        {
            return _db.WishlistItems.Any(wi => wi.AppUserId == user.Id && wi.ProductId == productId);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}