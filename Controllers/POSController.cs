// Controllers/POSController.cs
using EasyGamesProject.Data;
using EasyGamesProject.Models;
using EasyGamesProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

namespace EasyGamesProject.Controllers
{
    [Authorize(Roles = "Admin,Proprietor")] // Only Admins and Proprietors can use POS
    public class POSController : Controller
    {
        private readonly ApplicationDbContext _context;

        public POSController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper to map tier to discount rate
        private decimal GetDiscountRate(UserTier? tier) => tier switch
        {
            UserTier.Bronze => 5m,
            UserTier.Silver => 10m,
            UserTier.Gold => 15m,
            UserTier.Platinum => 20m,
            _ => 0m
        };

        // GET: POS
        public async Task<IActionResult> Index(int? shopId)
        {
            if (shopId == null)
            {
                // Redirect to the shop selection page
                return RedirectToAction(nameof(SelectShop));
            }

            // Load shop and verify it exists
            var shop = await _context.Shops.FindAsync(shopId);
            if (shop == null)
            {
                return NotFound();
            }

            // Create POS view model with available shop stock
            var model = new POSViewModel
            {
                AvailableStock = await _context.ShopStock
                    .Where(ss => ss.ShopId == shopId)
                    .Include(ss => ss.Stock)
                    .Select(ss => new SelectListItem
                    {
                        Value = ss.StockId.ToString(),
                        Text = $"{ss.Stock.Name} - ${ss.SellPrice:F2} (Available: {ss.Quantity})"
                    })
                    .ToListAsync()
            };

            ViewBag.ShopId = shopId;
            ViewBag.ShopName = shop.Name;

            return View(model);
        }

        // GET: POS/SelectShop
        public async Task<IActionResult> SelectShop()
        {
            var shops = await _context.Shops.ToListAsync();
            ViewBag.Shops = shops;
            return View(); // Views/POS/SelectShop.cshtml using default _Layout
        }

        // POST: POS/LookupCustomer
        [HttpPost]
        public async Task<IActionResult> LookupCustomer(string phone, int shopId)
        {
            var customer = await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNumber == phone);

            if (customer != null)
            {
                var rate = GetDiscountRate(customer.Tier);
                return Json(new
                {
                    success = true,
                    customerName = $"{customer.FirstName} {customer.LastName}",
                    customerTier = customer.Tier?.ToString() ?? "None",
                    tierDiscount = rate
                });
            }

            return Json(new { success = false, message = "Customer not found" });
        }

        // POST: POS/AddToCart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int stockId, int quantity, int shopId)
        {
            var shopStock = await _context.ShopStock
                .Include(ss => ss.Stock)
                .FirstOrDefaultAsync(ss => ss.ShopId == shopId && ss.StockId == stockId);

            if (shopStock == null)
            {
                return Json(new { success = false, message = "Item not found in shop inventory" });
            }

            var subtotal = shopStock.SellPrice * quantity;

            return Json(new
            {
                success   = true,
                itemName  = shopStock.Stock.Name,
                quantity,
                unitPrice = shopStock.SellPrice,
                subtotal
            });
        }

        // POST: POS/CompleteSale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteSale(POSViewModel model, int shopId)
        {
            // Deserialize cart from JSON
            model.CartItems = JsonSerializer
                .Deserialize<List<POSCartItem>>(model.CartJson ?? "[]")
                ?? new List<POSCartItem>();

            if (!model.CartItems.Any())
            {
                TempData["ErrorMessage"] = "Cart is empty";
                return RedirectToAction(nameof(Index), new { shopId });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                int? customerId = null;
                if (!string.IsNullOrWhiteSpace(model.CustomerPhone))
                {
                    var customer = await _context.Users
                        .FirstOrDefaultAsync(u => u.PhoneNumber == model.CustomerPhone);
                    customerId = customer?.UserId;
                }

                foreach (var item in model.CartItems)
                {
                    var shopStock = await _context.ShopStock
                        .Include(ss => ss.Stock)
                        .FirstOrDefaultAsync(ss => ss.ShopId == shopId && ss.StockId == item.StockId);

                    if (shopStock == null)
                    {
                        TempData["ErrorMessage"] = $"Item {item.ItemName} not found";
                        continue;
                    }

                    // Decrement stock and mark modified
                    shopStock.Quantity -= item.Quantity;
                    _context.Entry(shopStock).State = EntityState.Modified;

                    // Calculate pricing and discount
                    var unitPrice = shopStock.SellPrice;
                    var discountAmount = (unitPrice * item.Quantity) * (model.TierDiscount / 100);
                    var totalPrice = (unitPrice * item.Quantity) - discountAmount;

                    // Record sale
                    var sale = new Sale
                    {
                        ShopId         = shopId,
                        UserId         = customerId,
                        CustomerPhone  = model.CustomerPhone,
                        StockId        = item.StockId,
                        Quantity       = item.Quantity,
                        UnitPrice      = unitPrice,
                        DiscountAmount = discountAmount,
                        TotalPrice     = totalPrice,
                        SaleDate       = DateTime.Now,
                        SaleType       = "POS"
                    };
                    _context.Sales.Add(sale);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = "Sale completed successfully!";
                return RedirectToAction(nameof(Index), new { shopId });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = $"Error completing sale: {ex.Message}";
                return RedirectToAction(nameof(Index), new { shopId });
            }
        }

        // GET: POS/SalesHistory
        public async Task<IActionResult> SalesHistory(int shopId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _context.Sales
                .Where(s => s.ShopId == shopId)
                .Include(s => s.Stock)
                .Include(s => s.User)
                .OrderByDescending(s => s.SaleDate);

            if (fromDate.HasValue)
                query = (IOrderedQueryable<Sale>)query.Where(s => s.SaleDate >= fromDate.Value);

            if (toDate.HasValue)
                query = (IOrderedQueryable<Sale>)query.Where(s => s.SaleDate <= toDate.Value);

            var sales = await query.ToListAsync();

            ViewBag.ShopId = shopId;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;

            return View(sales);
        }
    }
}
