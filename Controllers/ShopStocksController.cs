// Created with the help of ChatGPT

using EasyGamesProject.Data;
using EasyGamesProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EasyGames.Controllers
{
    public class ShopStocksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopStocksController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var shopStocks = _context.ShopStock
                .Include(s => s.Shop)
                .Include(s => s.Stock);
            return View(await shopStocks.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var shopStock = await _context.ShopStock
                .Include(s => s.Shop)
                .Include(s => s.Stock)
                .FirstOrDefaultAsync(m => m.ShopStockId == id);

            if (shopStock == null) return NotFound();

            return View(shopStock);
        }

        public IActionResult Create()
        {
            ViewData["ShopId"] = new SelectList(_context.Shops, "ShopId", "Name");
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShopStockId,ShopId,StockId,Quantity")] ShopStock shopStock)
        {
            if (ModelState.IsValid)
            {
                var ownerStock = await _context.Stocks.FindAsync(shopStock.StockId);
                if (ownerStock == null || ownerStock.Quantity < shopStock.Quantity)
                {
                    ModelState.AddModelError("Quantity", "Insufficient owner stock quantity.");
                    ViewData["ShopId"] = new SelectList(_context.Shops, "ShopId", "Name", shopStock.ShopId);
                    ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Name", shopStock.StockId);
                    return View(shopStock);
                }

                ownerStock.Quantity -= shopStock.Quantity;
                shopStock.Source = ownerStock.Source;
                shopStock.BuyPrice = ownerStock.BuyPrice;
                shopStock.SellPrice = ownerStock.SellPrice;

                _context.Add(shopStock);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ShopId"] = new SelectList(_context.Shops, "ShopId", "Name", shopStock.ShopId);
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Name", shopStock.StockId);
            return View(shopStock);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var shopStock = await _context.ShopStock.FindAsync(id);
            if (shopStock == null) return NotFound();

            ViewData["ShopId"] = new SelectList(_context.Shops, "ShopId", "Name", shopStock.ShopId);
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Name", shopStock.StockId);
            return View(shopStock);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShopStockId,ShopId,StockId,Quantity")] ShopStock shopStock)
        {
            if (id != shopStock.ShopStockId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var ownerStock = await _context.Stocks.FindAsync(shopStock.StockId);
                    if (ownerStock == null) return NotFound();

                    var existingShopStock = await _context.ShopStock.AsNoTracking()
                        .FirstOrDefaultAsync(s => s.ShopStockId == id);
                    if (existingShopStock == null) return NotFound();

                    var quantityDiff = shopStock.Quantity - existingShopStock.Quantity;

                    if (quantityDiff > 0 && ownerStock.Quantity < quantityDiff)
                    {
                        ModelState.AddModelError("Quantity", "Insufficient owner stock to increase quantity.");
                        ViewData["ShopId"] = new SelectList(_context.Shops, "ShopId", "Name", shopStock.ShopId);
                        ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Name", shopStock.StockId);
                        return View(shopStock);
                    }
                    else if (quantityDiff > 0)
                    {
                        ownerStock.Quantity -= quantityDiff;
                    }
                    else if (quantityDiff < 0)
                    {
                        ownerStock.Quantity += (-quantityDiff);
                    }

                    _context.Update(shopStock);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ShopStock.Any(e => e.ShopStockId == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ShopId"] = new SelectList(_context.Shops, "ShopId", "Name", shopStock.ShopId);
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Name", shopStock.StockId);
            return View(shopStock);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var shopStock = await _context.ShopStock
                .Include(s => s.Shop)
                .Include(s => s.Stock)
                .FirstOrDefaultAsync(m => m.ShopStockId == id);
            if (shopStock == null) return NotFound();

            return View(shopStock);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shopStock = await _context.ShopStock.FindAsync(id);
            if (shopStock != null)
            {
                var ownerStock = await _context.Stocks.FindAsync(shopStock.StockId);
                if (ownerStock != null)
                {
                    ownerStock.Quantity += shopStock.Quantity;
                }

                _context.ShopStock.Remove(shopStock);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ShopStockExists(int id)
        {
            return _context.ShopStock.Any(e => e.ShopStockId == id);
        }
    }
}
