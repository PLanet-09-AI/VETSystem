using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // For ClaimsPrincipal extensions like FindFirstValue

using System.Linq;
using System.Threading.Tasks;
using BestReg.Data;

namespace BestReg.Controllers
{
    public class FarmManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FarmManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: FarmManager/Appointments
        public async Task<IActionResult> Appointments()
        {
            var appointments = await _context.VetAppointments
                .Where(a => !a.Canceled && !a.IsBooked) // Only show available appointments
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            return View(appointments);
        }

        // GET: FarmManager/BookAppointment
        public IActionResult BookAppointment()
        {
            // Fetch available time slots from the database
            ViewBag.AppointmentTypes = new SelectList(_context.AppointmentTypes, "Name", "Name");
            ViewBag.AvailableSlots = new SelectList(
                _context.VetAppointments
                    .Where(a => !a.Canceled && !a.IsBooked) // Only show unbooked slots
                    .Select(a => new
                    {
                        Id = a.Id,
                        Display = $"{a.StartTime:MM/dd/yyyy HH:mm} - {a.EndTime:HH:mm}"
                    }),
                "Id",
                "Display"
            );

            return View();
        }

        // POST: FarmManager/BookAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(int selectedSlotId)
        {
            var vetAppointment = await _context.VetAppointments.FindAsync(selectedSlotId);

            if (vetAppointment == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                vetAppointment.IsBooked = true;  // Mark the appointment as booked
                _context.Update(vetAppointment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Appointments));
            }

            ViewBag.AppointmentTypes = new SelectList(_context.AppointmentTypes, "Name", "Name");
            ViewBag.AvailableSlots = new SelectList(
                _context.VetAppointments
                    .Where(a => !a.Canceled && !a.IsBooked)
                    .Select(a => new
                    {
                        Id = a.Id,
                        Display = $"{a.StartTime:MM/dd/yyyy HH:mm} - {a.EndTime:HH:mm}"
                    }),
                "Id",
                "Display"
            );

            return View(vetAppointment);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int appointmentId)
        {
            var appointment = await _context.VetAppointments.FindAsync(appointmentId);
            if (appointment != null)
            {
                appointment.IsNotified = true;
                _context.Update(appointment);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Notifications));
        }


        public async Task<IActionResult> Notifications()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notifications = await _context.VetAppointments
                .Where(a => a.IsNotified == false && (a.IsDeclined || a.IsAccepted))
                .ToListAsync();

            return View(notifications);
        }
    }
}
