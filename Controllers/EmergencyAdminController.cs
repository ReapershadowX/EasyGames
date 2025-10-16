//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Identity;
//using EasyGamesProject.Data;
//using EasyGamesProject.Models;
//using System.Linq;

//namespace EasyGamesProject.Controllers
//{
//    public class EmergencyAdminController : Controller
//    {
//        private readonly ApplicationDbContext _context;

//        public EmergencyAdminController(ApplicationDbContext context)
//        {
//            _context = context;
//        }

//        // Show the form to create an admin
//        [HttpGet]
//        public IActionResult CreateAdmin()
//        {
//            return View();
//        }

//        // Handle form post to create an admin
//        [HttpPost]
//        public IActionResult CreateAdmin(string email, string password, string firstName, string lastName)
//        {
//            // Check if admin exists already
//            //if (_context.Users.Any(u => u.Role == "Admin"))
//            //{
//            //    return Content("Admin account exists. Creation disallowed.");
//            //}

//            var hasher = new PasswordHasher<User>();

//            var adminUser = new User
//            {
//                FirstName = firstName,
//                LastName = lastName,
//                Email = email,
//                Role = UserRole.Admin,
//                CreatedDate = System.DateTime.UtcNow
//            };

//            adminUser.Password = hasher.HashPassword(adminUser, password);

//            _context.Users.Add(adminUser);
//            _context.SaveChanges();

//            return RedirectToAction("Index", "Home");
//        }
//    }
//}
