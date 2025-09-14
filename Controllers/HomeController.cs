using System.Diagnostics;
using EasyGames.Models;
using Microsoft.AspNetCore.Mvc;
using EasyGamesProject.Data; // Add namespace for DbContext
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore; // Needed for Include

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
        // Landing page action; loads featured products including their images
        public async Task<IActionResult> Index()
        {
            bool canConnect = _context.Database.CanConnect();
            ViewBag.DatabaseStatus = canConnect ? "Connected to Database" : "Database Connection Failed";

            // Eager load related images for each stock product
            var featuredProducts = await _context.Stocks
                .Include(s => s.Images)
                .ToListAsync();

            // Pass products to the view
            return View(featuredProducts);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";
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
