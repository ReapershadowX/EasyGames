using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
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

        // Inject ApplicationDbContext via constructor for data access
        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Existing actions (Index, Details, Create, Edit, Delete, Register) unchanged

        // Login/Logout Actions

        // GET: Users/Login
        // Displays login form
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: Users/Login
        // Processes login credentials, validates user,
        // creates claims including role claim, and signs in
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            // Verify password
            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);
            if (verificationResult == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError("", "Invalid login attempt.");
                return View(model);
            }

            // Create claims for identity including role claim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email), // Can use user.FullName if added
                new Claim(ClaimTypes.Role, user.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true // optional, for "remember me"
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            return RedirectToAction("Index", "Home"); // Redirect after successful login
        }

        // POST: Users/Logout
        // Signs the user out
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        // Existing Registration actions, unchanged aside from redirect fix
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

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
                Role = "User", // default
                CreatedDate = DateTime.Now
            };

            user.Password = _passwordHasher.HashPassword(user, model.Password!);
            _context.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";
            return RedirectToAction("Login"); // Redirect to login within same controller
        }


        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
