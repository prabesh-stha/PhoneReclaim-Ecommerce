using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PhoneReclaim.Data.Models;
using PhoneReclaim.Models;
using Microsoft.EntityFrameworkCore;


namespace PhoneReclaim.Controllers
{

    [Authorize]
    public class ProductController : Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly PhoneReclaimDb _db;
        private readonly UserManager<AppUser> _usermanager;

        public ProductController(ILogger<ProductController> logger, PhoneReclaimDb db, UserManager<AppUser> usermanager)
        {
            _logger = logger;
            _db = db;
            _usermanager = usermanager;
        }

        public IActionResult Index()
        {
            List<Product> products = _db.Products.ToList();
            return View(products);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product, IFormFile imageFile)
        {
            var user = await _usermanager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }

            product.AppUserId = user.Id;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "productImage");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                product.ImagePath = "/productImage/" + uniqueFileName;
            }
            else
            {
                product.ImagePath = "/productImage/default.jpg"; // Set a default image path if no image is provided
            }

            _db.Products.Add(product);
            await _db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Product added successfully.";

            return RedirectToAction("Add", "Product");
        }
        public IActionResult Delete(Guid Id)
        {

            var productToDelete = _db.Products.FirstOrDefault(product => product.Id == Id);

            if (productToDelete == null)
            {
                return NotFound();
            }

            return View(productToDelete);

        }

        [HttpPost]
        public IActionResult Delete(Product product)
        {
            _db.Products.Remove(product);
            _db.SaveChanges();
            return RedirectToAction("UserProduct", "Account");

        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,Name,Brand,Condition,Price,Description,ImagePath,AppUserId")] Product product, IFormFile imageFile)
        {
            var user = await _usermanager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }

            var productToUpdate = _db.Products.FirstOrDefault(p => p.Id == product.Id);

            if (productToUpdate == null)
            {
                return NotFound();
            }

            productToUpdate.Name = product.Name;
            productToUpdate.Brand = product.Brand;
            productToUpdate.Condition = product.Condition;
            productToUpdate.Description = product.Description;
            productToUpdate.Price = product.Price;

            if (imageFile != null && imageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "productImage");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(fileStream);
                }

                productToUpdate.ImagePath = "/productImage/" + uniqueFileName;
            }

            _db.Products.Update(productToUpdate);
            _db.SaveChanges();

            return RedirectToAction("UserProduct", "Account");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}