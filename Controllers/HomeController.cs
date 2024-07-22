using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PhoneReclaim.Models;
using PhoneReclaim.Data;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PhoneReclaim.Data.Models;

namespace PhoneReclaim.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly PhoneReclaimDb _db;
    private readonly UserManager<AppUser> _usermanager;

    public HomeController(ILogger<HomeController> logger, PhoneReclaimDb db, UserManager<AppUser> userManager) // Modify the constructor
    {
        _logger = logger;
        _db = db;
        _usermanager = userManager;
    }
    public IActionResult Index()
    {
        List<Product> products = _db.Products.OrderByDescending(p => p.AddedDate).ToList();
        return View(products);
    }
    public List<Product> GetRecommendedProducts(Product product)
{

    var recommendedProducts = _db.Products
        .Where(p => p.Price >= product.Price - 100 && p.Price <= product.Price + 100 && p.Price != product.Price)
        .Take(4)
        .ToList();

    return recommendedProducts;
}
    public IActionResult GetProductById(Guid id)
    {

        var product = _db.Products.Include(p => p.AppUser).FirstOrDefault(p => p.Id == id);
        if (product == null)
        {
            return NotFound();
        }

        var recommendedProduct = GetRecommendedProducts(product);
        ViewBag.RecommendedProduct = recommendedProduct;

        ViewBag.UserName = product.AppUser?.Name;
        ViewBag.PhoneNumber = product.AppUser?.Phone;
        return View(product);
    }
    [HttpPost]
    public IActionResult Search(string searchTerm)
    {
        var products = _db.Products
            .Where(p => p.Name!.Contains(searchTerm))
            .ToList();
        ViewBag.search = searchTerm;

        return View(products);
    }
    [HttpPost]
    public IActionResult Filter(string selectedBrand)
    {
        var products = _db.Products.Include(p => p.AppUser).ToList();

        if (!string.IsNullOrEmpty(selectedBrand) && selectedBrand != "All Brands")
        {
            products = products.Where(p => p.Brand.ToString() == selectedBrand).ToList();
        }

        var brands = _db.Products.Select(p => p.Brand).Distinct().ToList();

        ViewBag.Brands = brands;
        ViewBag.SelectedBrand = selectedBrand;

        return View(products);
    }
    [HttpPost]
    public IActionResult FilterCategory(string category)
    {
        var products = _db.Products.Include(p => p.AppUser).ToList();
        if(!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Condition.ToString() == category).ToList();
            ViewBag.Category = category;
        }
        return View(products);
    }

    [HttpPost]
    public async Task<IActionResult> Toggle(Guid productId)
    {
        var user = await _usermanager.GetUserAsync(User);
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

        return RedirectToAction("Index", "Home");
    }
    public bool IsInWishlist(Guid productId)
    {
        var user = _usermanager.GetUserAsync(User).Result;
        return _db.WishlistItems.Any(wi => wi.AppUserId == user!.Id && wi.ProductId == productId);
    }

    



    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
