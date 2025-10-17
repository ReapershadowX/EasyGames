using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyGamesProject.Data;
using EasyGamesProject.Models;

namespace EasyGames.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public StocksController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: Stocks - Allow anonymous access for browsing stocks
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Stocks.ToListAsync());
        }

        // GET: Stocks/Details/5 - Allow anonymous access to view details of a stock item
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var stock = await _context.Stocks
                .Include(s => s.Images)
                .FirstOrDefaultAsync(m => m.StockId == id);

            if (stock == null)
                return NotFound();

            return View(stock);
        }

        // GET: Stocks/Create - Only Admins can access Create page
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.Categories = new List<string> { "Book", "Game", "Toy" };
            return View();
        }

        // POST: Stocks/Create - Only Admins can create stock items, and upload image files are processed
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Category,BuyPrice,SellPrice,Quantity,Source,Description")] Stock stock, List<IFormFile> ImageFiles)
        {
            if (ModelState.IsValid)
            {
                stock.CreatedDate = DateTime.Now;

                _context.Add(stock);
                await _context.SaveChangesAsync();

                // Process uploaded image files, if any
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
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = new List<string> { "Book", "Game", "Toy" };
            return View(stock);
        }

        // GET: Stocks/Edit/5 - Only Admins can access Edit page
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var stock = await _context.Stocks.FindAsync(id);
            if (stock == null)
                return NotFound();

            ViewBag.Categories = new List<string> { "Book", "Game", "Toy" };
            return View(stock);
        }

        // POST: Stocks/Edit/5 - Only Admins can edit stock including uploading new images
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockId,Name,Category,BuyPrice,SellPrice,Quantity,Source,Description,CreatedDate")] Stock stock, List<IFormFile> ImageFiles)
        {
            if (id != stock.StockId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stock);
                    await _context.SaveChangesAsync();

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

        // GET: Stocks/Delete/5 - Only Admins can view Delete confirmation page
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var stock = await _context.Stocks.FirstOrDefaultAsync(m => m.StockId == id);
            if (stock == null)
                return NotFound();

            return View(stock);
        }

        // POST: Stocks/Delete/5 - Only Admins can delete a stock
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stock = await _context.Stocks.FindAsync(id);
            if (stock != null)
                _context.Stocks.Remove(stock);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockId == id);
        }
    }
}
