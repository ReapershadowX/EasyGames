using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims; // For user claims access
using Microsoft.AspNetCore.Authorization; // Added for Authorize attribute
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EasyGamesProject.Data;
using EasyGamesProject.Models;

namespace EasyGames.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Scaffolded CRUD actions (Unchanged)

        // GET: ShoppingCarts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ShoppingCarts.Include(s => s.Stock).Include(s => s.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ShoppingCarts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) { return NotFound(); }

            var shoppingCart = await _context.ShoppingCarts
                .Include(s => s.Stock)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.CartId == id);

            if (shoppingCart == null) { return NotFound(); }

            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Create
        public IActionResult Create()
        {
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Category");
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email");
            return View();
        }

        // POST: ShoppingCarts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CartId,UserId,StockId,Quantity,DateAdded")] ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shoppingCart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Category", shoppingCart.StockId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", shoppingCart.UserId);
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) { return NotFound(); }

            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
            if (shoppingCart == null) { return NotFound(); }

            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Category", shoppingCart.StockId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", shoppingCart.UserId);
            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CartId,UserId,StockId,Quantity,DateAdded")] ShoppingCart shoppingCart)
        {
            if (id != shoppingCart.CartId) { return NotFound(); }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shoppingCart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShoppingCartExists(shoppingCart.CartId)) { return NotFound(); }
                    else { throw; }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["StockId"] = new SelectList(_context.Stocks, "StockId", "Category", shoppingCart.StockId);
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "Email", shoppingCart.UserId);
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) { return NotFound(); }

            var shoppingCart = await _context.ShoppingCarts
                .Include(s => s.Stock)
                .Include(s => s.User)
                .FirstOrDefaultAsync(m => m.CartId == id);

            if (shoppingCart == null) { return NotFound(); }

            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shoppingCart = await _context.ShoppingCarts.FindAsync(id);
            if (shoppingCart != null) { _context.ShoppingCarts.Remove(shoppingCart); }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ShoppingCartExists(int id)
        {
            return _context.ShoppingCarts.Any(e => e.CartId == id);
        }

        #endregion

        #region New User-Specific Shopping Cart Actions (Added)

        // Helper method to obtain current logged-in user's ID from claims
        private int GetCurrentUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        // Add item to cart - authenticated users only; uses POST method
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> AddToCart(int stockId, int quantity = 1)
        {
            if (quantity < 1) quantity = 1;

            int userId = GetCurrentUserId();

            var stock = await _context.Stocks.FindAsync(stockId);
            if (stock == null || stock.Quantity < quantity)
                return NotFound("Stock not available");

            var cartItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.UserId == userId && c.StockId == stockId);

            if (cartItem == null)
            {
                cartItem = new ShoppingCart
                {
                    UserId = userId,
                    StockId = stockId,
                    Quantity = quantity,
                    DateAdded = DateTime.Now
                };
                _context.ShoppingCarts.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
                _context.ShoppingCarts.Update(cartItem);
            }

            await _context.SaveChangesAsync();

            // Redirected to user's cart page after adding the item
            return RedirectToAction("ViewCart");
        }

        // View current user's shopping cart - authenticated users only
        [Authorize]
        public async Task<IActionResult> ViewCart()
        {
            int userId = GetCurrentUserId();

            var cartItems = await _context.ShoppingCarts
                .Where(c => c.UserId == userId)
                .Include(c => c.Stock)
                .ToListAsync();

            ViewBag.TotalPrice = cartItems.Sum(i => i.Stock!.Price * i.Quantity);

            return View(cartItems);
        }

        // Update quantity for a specific cart item - authenticated users only
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartId, int quantity)
        {
            if (quantity < 1) quantity = 1;


            int userId = GetCurrentUserId();

            var cartItem = await _context.ShoppingCarts
                .Include(c => c.Stock)
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == userId);

            if (cartItem == null || cartItem.Stock == null)
                return RedirectToAction("ViewCart");

            if (quantity > cartItem.Stock.Quantity)
                quantity = cartItem.Stock.Quantity;

            cartItem.Quantity = quantity;
            _context.ShoppingCarts.Update(cartItem);

            await _context.SaveChangesAsync();

            return RedirectToAction("ViewCart");
        }

        // Remove an item from user's cart - authenticated users only
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int cartId)
        {
            int userId = GetCurrentUserId();

            var cartItem = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.CartId == cartId && c.UserId == userId);

            if (cartItem != null)
            {
                _context.ShoppingCarts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("ViewCart");
        }

        // Show checkout page with current cart items - authenticated users only
        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            int userId = GetCurrentUserId();

            var cartItems = await _context.ShoppingCarts
                .Where(c => c.UserId == userId)
                .Include(c => c.Stock)
                .ToListAsync();

            ViewBag.TotalPrice = cartItems.Sum(i => i.Stock!.Price * i.Quantity);

            return View(cartItems);
        }

        // Confirm checkout and clear user's cart - authenticated users only
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ConfirmCheckout()
        {
            int userId = GetCurrentUserId();

            var cartItems = await _context.ShoppingCarts
                .Where(c => c.UserId == userId)
                .ToListAsync();

            _context.ShoppingCarts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            // Redirect user to homepage or a confirmation page after checkout
            return RedirectToAction("Index", "Home");
        }

        #endregion
    }
}
