using EasyGamesProject.Data;
using EasyGamesProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace EasyGames.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ShopController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Shop
        public async Task<IActionResult> Index()
        {
            // Eager load Proprietor navigation
            var shops = _context.Shops.Include(s => s.Proprietor);
            return View(await shops.ToListAsync());
        }

        // GET: Shop/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var shop = await _context.Shops.Include(s => s.Proprietor).FirstOrDefaultAsync(m => m.ShopId == id);
            if (shop == null) return NotFound();

            return View(shop);
        }

        // GET: Shop/Create
        public IActionResult Create()
        {
            // Populate proprietors dropdown filtered by role for usability
            ViewData["ProprietorId"] = new SelectList(_context.Users.Where(u => u.Role == UserRole.Proprietor), "UserId", "Email");
            return View();
        }

        // POST: Shop/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShopId,Name,Location,ProprietorId")] Shop shop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(shop);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProprietorId"] = new SelectList(_context.Users.Where(u => u.Role == UserRole.Proprietor), "UserId", "Email", shop.ProprietorId);
            return View(shop);
        }

        // GET: Shop/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var shop = await _context.Shops.FindAsync(id);
            if (shop == null) return NotFound();

            ViewData["ProprietorId"] = new SelectList(_context.Users.Where(u => u.Role == UserRole.Proprietor), "UserId", "Email", shop.ProprietorId);
            return View(shop);
        }

        // POST: Shop/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShopId,Name,Location,ProprietorId")] Shop shop)
        {
            if (id != shop.ShopId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(shop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Shops.Any(e => e.ShopId == shop.ShopId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProprietorId"] = new SelectList(_context.Users.Where(u => u.Role == UserRole.Proprietor), "UserId", "Email", shop.ProprietorId);
            return View(shop);
        }

        // GET: Shop/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var shop = await _context.Shops.Include(s => s.Proprietor).FirstOrDefaultAsync(m => m.ShopId == id);
            if (shop == null) return NotFound();

            return View(shop);
        }

        // POST: Shop/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var shop = await _context.Shops.FindAsync(id);
            if (shop != null)
            {
                _context.Shops.Remove(shop);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ShopExists(int id)
        {
            return _context.Shops.Any(e => e.ShopId == id);
        }
    }
}
