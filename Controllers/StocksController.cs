using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EasyGamesProject.Data;
using EasyGamesProject.Models;

namespace EasyGames.Controllers
{
    public class StocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject ApplicationDbContext via constructor for data access
        public StocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Stocks
        // Retrieves all stock items asynchronously and passes to the Index view
        public async Task<IActionResult> Index()
        {
            return View(await _context.Stocks.ToListAsync());
        }

        // GET: Stocks/Details/5
        // Displays details of a specific stock item by ID; returns 404 if not found
        public async Task<IActionResult> Details(int? id)
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

        // GET: Stocks/Create
        // Returns the view to create a new stock item
        public IActionResult Create()
        {
            return View();
        }

        // POST: Stocks/Create
        // Protects from overposting attacks by binding only allowed properties
        // Sets CreatedDate server-side before saving
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Category,Price,Quantity,Description")] Stock stock)
        {
            if (ModelState.IsValid)
            {
                stock.CreatedDate = DateTime.Now; // Set CreatedDate here to current time
                _context.Add(stock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stock);
        }

        // GET: Stocks/Edit/5
        // Returns the view to edit an existing stock item by ID
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
            return View(stock);
        }

        // POST: Stocks/Edit/5
        // Protects from overposting by binding allowed properties including the ID and CreatedDate
        // Handles concurrency exceptions gracefully
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockId,Name,Category,Price,Quantity,Description,CreatedDate")] Stock stock)
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
            return View(stock);
        }

        // GET: Stocks/Delete/5
        // Returns the view to confirm deletion of stock item by ID
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
        // Removes the specified stock item and saves changes
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

        // Utility method to check if a stock with the given ID exists
        private bool StockExists(int id)
        {
            return _context.Stocks.Any(e => e.StockId == id);
        }
    }
}
