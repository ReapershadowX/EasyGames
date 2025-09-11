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

        // GET: /Home/Index or /
        // Landing page action; checks database connectivity status and passes message to view
        public IActionResult Index()
        {
            bool canConnect = _context.Database.CanConnect();
            ViewBag.DatabaseStatus = canConnect ? "Connected to Database" : "Database Connection Failed";
            return View();
        }

        // GET: /Home/About
        // Displays the About page with informational message
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        // GET: /Home/Contact
        // Displays the Contact page with informational message
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
            return View();
        }

        // GET: /Home/Privacy
        // Privacy policy page (existing)
        public IActionResult Privacy()
        {
            return View();
        }

        // Error page with no caching; uses ErrorViewModel for tracking
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
