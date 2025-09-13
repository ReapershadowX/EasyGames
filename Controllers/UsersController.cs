using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyGamesProject.Data;
using EasyGamesProject.Models;
using EasyGamesProject.ViewModels; // For RegisterViewModel
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

        // GET: Users
        // Retrieves all users asynchronously and passes to the Index view
        public async Task<IActionResult> Index()
        {
            return View(await _context.Users.ToListAsync());
        }

        // GET: Users/Details/5
        // Displays details of a specific user by ID; returns 404 if not found
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        // Returns the view to create a new user (admin use)
        public IActionResult Create()
        {
            ViewBag.Roles = new List<string> { "User", "Admin", "Moderator" };
            return View();
        }

        // POST: Users/Create
        // Protects from overposting attacks by binding only allowed properties
        // Hashes user password before saving
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Password,Role")] User user)
        {
            if (ModelState.IsValid)
            {
                user.CreatedDate = DateTime.Now; // Set creation date server-side
                // Hash user.Password for security before saving
                user.Password = _passwordHasher.HashPassword(user, user.Password);
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Users/Edit/5
        // Returns the view to edit an existing user by ID
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
            ViewBag.Roles = new List<string> { "User", "Admin", "Moderator" };
            return View(user);
        }

        // POST: Users/Edit/5
        // Protects from overposting by binding allowed properties including ID and CreatedDate
        // Hashes password if modified before saving
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,FirstName,LastName,Email,Password,Role,CreatedDate")] User user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    // Hash user.Password before saving edits to ensure security
                    user.Password = _passwordHasher.HashPassword(user, user.Password);
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
            return View(user);
        }

        // GET: Users/Delete/5
        // Returns the view to confirm deletion of a user by ID
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        // Removes the specified user and saves changes
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

        // Utility method to check if a user with the given ID exists
        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }

        // GET: Users/Register
        // Displays the registration form for public user signup
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: Users/Register
        // Processes registration data, validates, checks for email duplicates,
        // hashes password, and saves new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Server-side email uniqueness check
            bool emailExists = _context.Users.Any(u => u.Email == model.Email);
            if (emailExists)
            {
                ModelState.AddModelError("Email", "This email is already registered.");
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName!, // Assumed non-null due to ModelState.IsValid check
                LastName = model.LastName!, // Assumed non-null due to ModelState.IsValid check
                Email = model.Email!, // Assumed non-null and valid email because of validation attributes
                Role = "User", // Default role for new registrations
                CreatedDate = DateTime.Now
            };

            // Hash the password before saving
            // Password is non-null here because ModelState.IsValid ensures validation passed
            user.Password = _passwordHasher.HashPassword(user, model.Password!);

            user.Password = _passwordHasher.HashPassword(user, model.Password!);

            _context.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";

            // Redirect to Login page or home after successful registration

            // TO TO:
            return RedirectToAction("Login", "Account"); // Adjust as per your routing
        }
    }
}
