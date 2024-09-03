
﻿using Amazon.SimpleSystemsManagement.Model;
using BestReg.Data;
using BestReg.Models; // Add this to use the InventoryItem, OrderItem, and SupplierOrder classes
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using InventoryItem = BestReg.Data.InventoryItem;
=======
﻿using Microsoft.AspNetCore.Authorization;
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


        // Hardcoded balance for the supplier
        private decimal supplierBalance = 5000m;

        // Private method to get hardcoded inventory items
        private List<BestReg.Data.InventoryItem> GetHardcodedInventoryItems()
        {
            return new List<BestReg.Data.InventoryItem>
    {
        new BestReg.Data.InventoryItem { Id = 1, Name = "Corn", PricePerUnit = 10.5m, QuantityInStock = 1000 },
        new BestReg.Data.InventoryItem { Id = 2, Name = "Wheat", PricePerUnit = 8.75m, QuantityInStock = 500 },
        new BestReg.Data.InventoryItem { Id = 3, Name = "Soybeans", PricePerUnit = 12.3m, QuantityInStock = 750 },
        new BestReg.Data.InventoryItem { Id = 4, Name = "Barley", PricePerUnit = 7.2m, QuantityInStock = 300 },
        new BestReg.Data.InventoryItem { Id = 5, Name = "Oats", PricePerUnit = 6.5m, QuantityInStock = 450 }
    };
        }


        // Private method to get hardcoded supplier orders
        private List<SupplierOrder> GetHardcodedSupplierOrders()
        {
            var inventoryItems = GetHardcodedInventoryItems();

            return new List<SupplierOrder>
            {
                new SupplierOrder
                {
                    Id = 1,
                    SupplierName = "Supplier A",
                    OrderDate = DateTime.Now.AddDays(-10),
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Id = 1,
                            Item = inventoryItems.First(i => i.Id == 1), // Corn
                            QuantityOrdered = 100
                        },
                        new OrderItem
                        {
                            Id = 2,
                            Item = inventoryItems.First(i => i.Id == 2), // Wheat
                            QuantityOrdered = 50
                        }
                    }
                },
                new SupplierOrder
                {
                    Id = 2,
                    SupplierName = "Supplier B",
                    OrderDate = DateTime.Now.AddDays(-5),
                    Items = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Id = 3,
                            Item = inventoryItems.First(i => i.Id == 3), // Soybeans
                            QuantityOrdered = 200
                        }
                    }
                }
            };
        }

        // Dashboard Action
        [Authorize]
        public IActionResult Dashboard()
        {
            var supplierOrders = GetHardcodedSupplierOrders(); // This returns a List<SupplierOrder>
            ViewBag.CurrencyFormat = new CultureInfo("en-ZA");
            return View(supplierOrders); // Pass the correct model type
        }

        // Inventory Management Action
        public IActionResult Inventory()
        {
            var inventoryItems = GetHardcodedInventoryItems();

            // Format the price per unit in Rands
            ViewBag.CurrencyFormat = new CultureInfo("en-ZA");

            return View(inventoryItems);
        }

        // CRUD Operations for Inventory Management

        // GET: InventoryManagement/Index
        public async Task<IActionResult> Index()
        {
            var items = await _context.InventoryItems.ToListAsync();
            ViewBag.CurrencyFormat = new CultureInfo("en-ZA");

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


        // CRUD Operations for Supplier Orders

        // GET: SupplierOrders/Create
        public async Task<IActionResult> SupplierCreate()
        {
            var items = await _context.InventoryItems.ToListAsync();
            ViewBag.Items = items;

            ViewBag.CurrencyFormat = new CultureInfo("en-ZA");
            ViewBag.SupplierBalance = supplierBalance; // Pass the balance to the view
            return View();
        }

        // POST: SupplierOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupplierCreate([Bind("SupplierName,Items")] SupplierOrder order, List<int> selectedItems, List<int> quantities)
        {
            if (ModelState.IsValid)
            {
                decimal totalOrderCost = 0m;

                // Calculate the total cost of the order
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    var item = await _context.InventoryItems.FindAsync(selectedItems[i]);
                    if (item != null)
                    {
                        totalOrderCost += item.PricePerUnit * quantities[i];
                    }
                }

                // Check if the supplier has enough balance
                if (totalOrderCost > supplierBalance)
                {
                    ModelState.AddModelError("", "Insufficient balance to complete the order.");
                    var items = await _context.InventoryItems.ToListAsync();
                    ViewBag.Items = items;
                    ViewBag.CurrencyFormat = new CultureInfo("en-ZA");
                    ViewBag.SupplierBalance = supplierBalance;
                    return View(order);
                }

                // Deduct the total cost from the supplier's balance
                supplierBalance -= totalOrderCost;

                // Deduct the quantities from the inventory
                for (int i = 0; i < selectedItems.Count; i++)
                {
                    var item = await _context.InventoryItems.FindAsync(selectedItems[i]);
                    if (item != null)
                    {
                        item.QuantityInStock -= quantities[i];
                        _context.Update(item);
                    }
                }

                // Add the order to the database
                _context.SupplierOrders.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Dashboard));
            }

            // If the model state is invalid, repopulate the view with the necessary data
            var inventoryItems = await _context.InventoryItems.ToListAsync();
            ViewBag.Items = inventoryItems;
            ViewBag.CurrencyFormat = new CultureInfo("en-ZA");
            ViewBag.SupplierBalance = supplierBalance;
            return View(order);
        }

    }
}

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

