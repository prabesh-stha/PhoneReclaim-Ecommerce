using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using PhoneReclaim.Data.Models;

namespace PhoneReclaim.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;

        private readonly SignInManager<AppUser> signInManager;
        private readonly UserManager<AppUser> userManager;

        private readonly PhoneReclaimDb _db;


        public AccountController(ILogger<AccountController> logger, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, PhoneReclaimDb db)
        {
            _logger = logger;
            _db = db;
            this.signInManager = signInManager;
            this.userManager = userManager;
        }


        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(login.UserName!, login.Password!, login.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(login);
            }
            return View(login);
        }
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM register, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "userImage");
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

                    register.ImageUrl = "/userImage/" + uniqueFileName;
                }
                else
                {
                    register.ImageUrl = "/userImage/default.jpg"; // Set a default image path if no image is provided
                }
                AppUser user = new()
                {
                    Name = register.Name,
                    UserName = register.Email,
                    Email = register.Email,
                    Phone = register.Phone,
                    ImageUrl = register.ImageUrl
                };
                var result = await userManager.CreateAsync(user, register.Password!);

                if (result.Succeeded)
                {
                    await signInManager.SignInAsync(user, false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(register);
        }
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult UserProduct()
        {
            var user = userManager.GetUserAsync(User).Result;
            if (user == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if user is not authenticated
            }

            var products = _db.Products.Where(p => p.AppUserId == user.Id).ToList();
            ViewBag.userName = user.Name.Substring(0,user.Name.IndexOf(' '));

            return View(products);
        }
        public async Task<IActionResult> Profile()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // var userProfileViewModel = new ProfileVM
            // {
            //     UserId = user.Id,
            //     Name = user.Name,
            //     Email = user.Email,
            //     Phone = user.Phone,
            //     ImageUrl = user.ImageUrl
            // };

            // return View(userProfileViewModel);
            return View(user);
        }
        public async Task<IActionResult> Wishlist()
        {
            var user = await userManager.GetUserAsync(User);
            var wishlistItems = await _db.WishlistItems
                .Include(wi => wi.Product)
                .Where(wi => wi.AppUserId == user!.Id)
                .ToListAsync();
            return View(wishlistItems);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Populate a ViewModel or pass the user object to the view
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Phone,ImageUrl")] AppUser user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await userManager.FindByIdAsync(id);
                    existingUser.Name = user.Name;
                    existingUser.Phone = user.Phone;
                    await userManager.UpdateAsync(existingUser);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile","Account");
            }
            return View(user);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirm(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            await signInManager.SignOutAsync();
            await userManager.DeleteAsync(user);
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(string id)
        {
            return userManager.FindByIdAsync(id).Result != null;
        }

        public async Task<IActionResult> ChangeProfileImage(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public async Task<IActionResult> ChangeProfileImageFinal(string id, IFormFile imageFile)
        {

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {

                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "userImage");
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

                    user.ImageUrl = "/userImage/" + uniqueFileName;
                }
                else
                {
                    user.ImageUrl = "/userImage/default.jpg"; // Set a default image path if no image is provided
                }
                await userManager.UpdateAsync(user);
                return RedirectToAction("Profile", "Account");
                   
            }
             return RedirectToAction("Profile", "Account");
        }
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.GetUserAsync(User);
                if (user == null)
                {
                    return NotFound();
                }

                var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await signInManager.RefreshSignInAsync(user);
                    await signInManager.SignOutAsync();
                    return RedirectToAction("Login", "Account");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}