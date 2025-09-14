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

        // Other GET actions omitted for brevity...

        // POST: Stocks/Create with image upload handling
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Category,Price,Quantity,Description")] Stock stock, List<IFormFile> ImageFiles)
        {
            if (ModelState.IsValid)
            {
                stock.CreatedDate = DateTime.Now;
                _context.Add(stock);
                await _context.SaveChangesAsync(); // Save to generate StockId

                if (ImageFiles != null && ImageFiles.Count > 0)
                {
                    foreach (var file in ImageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/stocks");
                            Directory.CreateDirectory(uploadsFolder);

                            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            var stockImage = new StockImage
                            {
                                StockId = stock.StockId,
                                ImageUrl = "/images/stocks/" + uniqueFileName
                            };
                            _context.StockImages.Add(stockImage);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Categories = new List<string> { "Book", "Game", "Toy" };
            return View(stock);
        }

        // POST: Stocks/Edit with image upload handling
        [HttpPost]
        [ValidateAntiForgeryToken]
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

                    if (ImageFiles != null && ImageFiles.Count > 0)
                    {
                        foreach (var file in ImageFiles)
                        {
                            if (file.Length > 0)
                            {
                                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/stocks");
                                Directory.CreateDirectory(uploadsFolder);

                                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                                using (var stream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(stream);
                                }

                                var stockImage = new StockImage
                                {
                                    StockId = stock.StockId,
                                    ImageUrl = "/images/stocks/" + uniqueFileName
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

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockId == id);
        }
    }
}
