using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;           // For IWebHostEnvironment
using Microsoft.AspNetCore.Http;               // For IFormFile
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyGamesProject.Data;
using EasyGamesProject.Models;
using System.IO;                     // For file handling
using Microsoft.AspNetCore.Http;     // For IFormFile

namespace EasyGames.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment; // For file storage path

        // Inject ApplicationDbContext and IWebHostEnvironment via constructor
        public StocksController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Stocks
        public async Task<IActionResult> Index()
        {
            return View(await _context.Stocks.ToListAsync());
        }

        // GET: Stocks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stock = await _context.Stocks
                .Include(s => s.Images) // Include related images
                .FirstOrDefaultAsync(m => m.StockId == id);
            if (stock == null)
            {
                return NotFound();
            }
            return View(stock);
        }

        // GET: Stocks/Create
        public IActionResult Create()
        {
            ViewBag.Categories = new List<string> { "Book", "Game", "Toy" };
            return View();
        }

        // POST: Stocks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Added parameter ImageFiles to accept uploaded images
        public async Task<IActionResult> Create([Bind("Name,Category,Price,Quantity,Description")] Stock stock, List<IFormFile> ImageFiles)
        {
            if (ModelState.IsValid)
            {
                stock.CreatedDate = DateTime.Now;
                _context.Add(stock);
                await _context.SaveChangesAsync();

                // Handle image files if any uploaded
                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var image in ImageFiles)
                    {
                        if (image.Length > 0)
                        {
                            // Create unique filename using GUID to avoid conflicts
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                            // Define the path to save the file
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                            // Save the uploaded file to the server
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            // Create StockImage record linked to this stock
                            var stockImage = new StockImage
                            {
                                StockId = stock.StockId,
                                ImageUrl = "/images/" + fileName
                            };
                            _context.StockImages.Add(stockImage);
                        }
                    }
                    // Save all StockImage records to the database
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new List<string> { "Book", "Game", "Toy" };
            return View(stock);
        }

        // GET: Stocks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
            {
                return NotFound();
            }
            ViewBag.Categories = new List<string> { "Book", "Game", "Toy" };
            return View(stock);
        }

        // POST: Stocks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // Added ImageFiles parameter to accept uploaded images during edit
        public async Task<IActionResult> Edit(int id, [Bind("StockId,Name,Category,Price,Quantity,Description,CreatedDate")] Stock stock, List<IFormFile> ImageFiles)
        {
            if (id != stock.StockId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stock);
                    await _context.SaveChangesAsync();

                    // Handle new image uploads if any
                    if (ImageFiles != null && ImageFiles.Count > 0)
                    {
                        foreach (var image in ImageFiles)
                        {
                            if (image.Length > 0)
                            {
                                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await image.CopyToAsync(stream);
                                }

                                var stockImage = new StockImage
                                {
                                    StockId = stock.StockId,
                                    ImageUrl = "/images/" + fileName
                                };
                                _context.StockImages.Add(stockImage);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockExists(stock.StockId))
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
            ViewBag.Categories = new List<string> { "Book", "Game", "Toy" };
            return View(stock);
        }

        // GET: Stocks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var stock = await _context.Stocks
                .FirstOrDefaultAsync(m => m.StockId == id);
            if (stock == null)
            {
                return NotFound();
            }
            return View(stock);
        }

        // POST: Stocks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock != null)
            {
                _context.Stocks.Remove(stock);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockId == id);
        }
    }
}
