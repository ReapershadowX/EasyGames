using System.Diagnostics;
using EasyGames.Models;
using Microsoft.AspNetCore.Mvc;
using EasyGamesProject.Data; // Add namespace for DbContext
using Microsoft.Extensions.Logging;

namespace EasyGames.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context; // Add DbContext field

        // Modify constructor to inject both logger and ApplicationDbContext
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context; // Inject the database context via DI
        }

        public IActionResult Index()
        {
            // Test the database connection; canConnect will be true if connection succeeds
            bool canConnect = _context.Database.CanConnect();
            ViewBag.DatabaseStatus = canConnect ? "Connected to Database" : "Database Connection Failed";

            return View();
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
}
