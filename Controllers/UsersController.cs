using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization; // For Authorize attribute
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyGamesProject.Data;
using EasyGamesProject.Models;
using EasyGamesProject.ViewModels; // For RegisterViewModel and LoginViewModel
using Microsoft.AspNetCore.Identity; // For PasswordHasher

namespace EasyGames.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Only Admins can see the users list
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // Admin, Proprietor & Customers can see details
        [Authorize(Roles = "Admin,Proprietor,Customer")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // Admin-only. Show create user form
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Roles = new List<string> { "Admin", "Proprietor", "Customer" };
            return View();
        }

        // Admin-only. Create new user
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Password,Role,PhoneNumber,Tier")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedDate = DateTime.Now;
                // Parse Role enum from string
                if (!Enum.TryParse(user.Role.ToString(), out UserRole parsedRole))
                {
                    parsedRole = UserRole.Customer; // Default to Customer if invalid
                }
                user.Role = parsedRole;

                user.Password = _passwordHasher.HashPassword(user, user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = new List<string> { "Admin", "Proprietor", "Customer" };
            return View(user);
        }

        // Admin & Proprietor can edit users
        [Authorize(Roles = "Admin,Proprietor")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewBag.Roles = new List<string> { "Admin", "Proprietor", "Customer" };
            return View(user);
        }

        // Admin-only submit user edit
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Email,Password,Role,Tier,PhoneNumber,CreatedDate")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            // Allow edit without requiring password
            ModelState.Remove(nameof(user.Password));

            if (ModelState.IsValid)
            {
                try
                {
                    var existingUser = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserId == user.UserId);
                    if (existingUser == null)
                    {
                        return NotFound();
                    }

                    if (!Enum.TryParse(user.Role.ToString(), out UserRole parsedRole))
                    {
                        parsedRole = UserRole.Customer;
                    }
                    user.Role = parsedRole;

                    // Prevent double hashing of password
                    if (string.IsNullOrWhiteSpace(user.Password) || user.Password == existingUser.Password)
                    {
                        // Password not changed, keep existing hashed password
                        user.Password = existingUser.Password;
                    }
                    else
                    {
                        // Password changed, hash new password input
                        user.Password = _passwordHasher.HashPassword(user, user.Password);
                    }

                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Roles = new List<string> { "Admin", "Proprietor", "Customer" };
            return View(user);
        }

        // Admin-only user deletion page
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FirstOrDefaultAsync(m => m.UserId == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // Admin-only confirm user delete
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Login page, accessible without authentication
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // Login form submit - validate and sign in user
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password!);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            // Add UserId claim for identification in other parts of the app
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // Added claim for UserId
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Role, user.Role.ToString()) // Convert enum to string
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home");
        }

        // Log out action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Registration page - accessible without authentication
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // Registration form submit - create new user
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            bool emailExists = _context.Users.Any(u => u.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName!,
                LastName = model.LastName!,
                Email = model.Email!,
                Role = UserRole.Customer, // default role
                CreatedDate = DateTime.Now
            };

            user.Password = _passwordHasher.HashPassword(user, model.Password!);
            _context.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";
            return RedirectToAction("Login");
        }

        // Utility check if user exists by ID
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
