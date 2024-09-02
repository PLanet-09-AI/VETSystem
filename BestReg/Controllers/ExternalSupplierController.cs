using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using BestReg.Data;

namespace BestReg.Controllers
{
    [Authorize(Roles = "ExternalSupplier")]
    public class ExternalSupplierController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ExternalSupplierController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Dashboard Action
        public async Task<IActionResult> Dashboard()
        {
            var orders = await _context.SupplierOrders
                .Include(o => o.Items)
                .ToListAsync();
            return View(orders);
        }

        // Inventory Management Action
        public async Task<IActionResult> Index()
        {
            var items = await _context.InventoryItems.ToListAsync();
            return View(items);
        }

        // GET: InventoryManagement/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: InventoryManagement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,PricePerUnit")] InventoryItem item)
        {
            if (ModelState.IsValid)
            {
                _context.Add(item);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        // GET: InventoryManagement/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: InventoryManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PricePerUnit")] InventoryItem item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryItemExists(item.Id))
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
            return View(item);
        }

        // GET: InventoryManagement/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.InventoryItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: InventoryManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Check if InventoryItem exists
        private bool InventoryItemExists(int id)
        {
            return _context.InventoryItems.Any(e => e.Id == id);
        }

        // GET: SupplierOrders/Create
        public async Task<IActionResult> SupplierCreate()
        {
            var items = await _context.InventoryItems.ToListAsync();
            ViewBag.Items = items;
            return View();
        }


        // POST: SupplierOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupplierCreate([Bind("SupplierName")] SupplierOrder order)
        {
            if (ModelState.IsValid)
            {
                _context.SupplierOrders.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }
            var items = await _context.InventoryItems.ToListAsync();
            ViewBag.Items = items;
            return View(order);
        }

        public async Task<IActionResult> SupplierEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.SupplierOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            var items = await _context.InventoryItems.ToListAsync();
            ViewBag.Items = items;
            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupplierEdit(int id, [Bind("Id,SupplierName")] SupplierOrder order)
        {
            if (id != order.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await SupplierOrderExists(order.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Dashboard));
            }
            var items = await _context.InventoryItems.ToListAsync();
            ViewBag.Items = items;
            return View(order);
        }

        private async Task<bool> SupplierOrderExists(int id)
        {
            return await _context.SupplierOrders.AnyAsync(e => e.Id == id);
        }


        // GET: SupplierOrders/Delete/5
        public async Task<IActionResult> SupplierDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.SupplierOrders
                .FirstOrDefaultAsync(m => m.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: SupplierOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupplierDeleteConfirmed(int id)
        {
            var order = await _context.SupplierOrders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.SupplierOrders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Dashboard));
        }

        // Check if SupplierOrder exists
        private bool CheckIfSupplierOrderExists(int id)
        {
            return _context.SupplierOrders.Any(e => e.Id == id);
        }

        // GET: InventoryManagement/Edit/5
        public async Task<IActionResult> InventoryEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            return View(item);
        }

        // POST: InventoryManagement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InventoryEdit(int id, [Bind("Id,Name,PricePerUnit")] InventoryItem item)
        {
            if (id != item.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(item);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InventoryItemExists(item.Id))
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
            return View(item);
        }

        // GET: InventoryManagement/Delete/5
        public async Task<IActionResult> InventoryDelete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var item = await _context.InventoryItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        // POST: InventoryManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InventoryDeleteConfirmed(int id)
        {
            var item = await _context.InventoryItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            _context.InventoryItems.Remove(item);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Check if InventoryItem exists
        private bool IsInventoryItemExists(int id)
        {
            return _context.InventoryItems.Any(e => e.Id == id);
        }
    }
}

