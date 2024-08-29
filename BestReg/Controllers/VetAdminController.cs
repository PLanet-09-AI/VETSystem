using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BestReg.Data;
using Microsoft.AspNetCore.Identity;

namespace BestReg.Controllers
{
    public class VetAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VetAdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: VetAdmin
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.VetAppointments
                .OrderBy(a => a.StartTime)
                .ToListAsync();
            return View(appointments);
        }

        // GET: VetAdmin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vetAppointment = await _context.VetAppointments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vetAppointment == null)
            {
                return NotFound();
            }

            return View(vetAppointment);
        }
        // GET: VetAdmin/Create
        public IActionResult Create()
        {
            ViewBag.AppointmentTypes = new SelectList(_context.AppointmentTypes, "Name", "Name");
            return View();
        }

        // POST: VetAdmin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartTime,EndTime,AppointmentType")] VetAppointment vetAppointment)
        {
            if (ModelState.IsValid)
            {
                // Automatically set the VetAdminId to the currently logged-in user's ID
                var userId = _userManager.GetUserId(User);
                vetAppointment.VetAdminId = userId;

                // Add the new appointment to the database
                _context.Add(vetAppointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Re-populate the dropdown list if model state is invalid
            ViewBag.AppointmentTypes = new SelectList(_context.AppointmentTypes, "Name", "Name");
            return View(vetAppointment);
        }

        // GET: VetAdmin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vetAppointment = await _context.VetAppointments.FindAsync(id);
            if (vetAppointment == null)
            {
                return NotFound();
            }

            ViewBag.AppointmentTypes = new SelectList(_context.AppointmentTypes, "Name", "Name", vetAppointment.AppointmentType);
            return View(vetAppointment);
        }

        // POST: VetAdmin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime,AppointmentType,VetAdminId")] VetAppointment vetAppointment)
        {
            if (id != vetAppointment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vetAppointment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VetAppointmentExists(vetAppointment.Id))
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

            ViewBag.AppointmentTypes = new SelectList(_context.AppointmentTypes, "Name", "Name", vetAppointment.AppointmentType);
            return View(vetAppointment);
        }

        // GET: VetAdmin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vetAppointment = await _context.VetAppointments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vetAppointment == null)
            {
                return NotFound();
            }

            return View(vetAppointment);
        }

        // POST: VetAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vetAppointment = await _context.VetAppointments.FindAsync(id);
            if (vetAppointment != null)
            {
                _context.VetAppointments.Remove(vetAppointment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: VetAdmin/Cancel/5
        public async Task<IActionResult> Cancel(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vetAppointment = await _context.VetAppointments.FindAsync(id);
            if (vetAppointment == null)
            {
                return NotFound();
            }

            return View(vetAppointment); // This will look for the Cancel.cshtml view
        }

        // POST: VetAdmin/CancelConfirmed/5
        [HttpPost, ActionName("CancelConfirmed")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelConfirmed(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vetAppointment = await _context.VetAppointments.FindAsync(id);
            if (vetAppointment == null)
            {
                return NotFound();
            }

            vetAppointment.Canceled = true; // Mark the appointment as canceled
            _context.Update(vetAppointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect back to the index view after cancellation
        }

        // GET: VetAdmin/Decline/5
        public async Task<IActionResult> Decline(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vetAppointment = await _context.VetAppointments.FindAsync(id);
            if (vetAppointment == null)
            {
                return NotFound();
            }

            return View(vetAppointment); // Display the decline view with the appointment details
        }

        // POST: VetAdmin/Decline
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Decline(int id, string declineReason)
        {
            var vetAppointment = await _context.VetAppointments.FindAsync(id);
            if (vetAppointment == null)
            {
                return NotFound();
            }

            vetAppointment.IsDeclined = true;
            vetAppointment.DeclineReason = declineReason; // Store the reason for declining
            vetAppointment.IsNotified = false; // Mark it as not notified so it appears in the farm manager's notifications

            _context.Update(vetAppointment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); // Redirect back to the index view after saving changes
        }

        private bool VetAppointmentExists(int id)
        {
            return _context.VetAppointments.Any(e => e.Id == id);
        }
    }
}
