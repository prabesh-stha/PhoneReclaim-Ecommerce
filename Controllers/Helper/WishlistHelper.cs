using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using PhoneReclaim.Data.Models;

public static class WishlistHelper
{
    public static bool IsInWishlist(Guid productId, UserManager<AppUser> userManager, PhoneReclaimDb db, ClaimsPrincipal user)
{
    if (user == null || !user.Identity.IsAuthenticated)
    {
        return false;
    }

    var appUser = userManager.GetUserAsync(user).Result;
    if (appUser == null)
    {
        return false;
    }

    return db.WishlistItems.Any(wi => wi.AppUserId == appUser.Id && wi.ProductId == productId);
}

}