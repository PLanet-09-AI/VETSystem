using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BestReg.Data;
using Microsoft.Extensions.Logging;
using BestReg.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace BestReg.Controllers
{
    [Authorize(Roles = "FarmWorker,Veterinarian")]
    public class AnimalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnimalsController> _logger;

        public AnimalsController(ApplicationDbContext context, ILogger<AnimalsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Animals
        public async Task<IActionResult> Index()
        {
            return View(await _context.Animals.ToListAsync());
        }

        // GET: Animals/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Species,DateOfBirth,Status")] Animal animal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(animal);
        }


        // GET: Animals/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }

        // POST: Animals/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Species,DateOfBirth,Status")] Animal animal)
        {
            if (id != animal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(animal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExists(animal.Id))
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
            return View(animal);
        }


        // GET: Animals/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animal == null)
            {
                return NotFound();
            }

            return View(animal);
        }

        // POST: Animals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }

        // Method for the farm manager to book appointments with the animal added
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookAppointment(BookAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Use model.SelectedAnimalId to get the selected animal's ID
                var animal = await _context.Animals.FindAsync(model.SelectedAnimalId);
                if (animal == null)
                {
                    return NotFound();
                }

                // Create and save the appointment
                var appointment = new VetAppointment
                {
                    AnimalId = model.SelectedAnimalId,
                    AppointmentType = model.AppointmentType,
                    StartTime = model.StartTime,
                    EndTime = model.EndTime,
                    VetAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier), // Automatically set VetAdminId
                    IsBooked = true,
                    Canceled = false,
                    IsDeclined = false,
                    IsNotified = false
                };

                _context.VetAppointments.Add(appointment);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Redirect to a relevant page after booking
            }

            // If the model state is invalid, repopulate the Animals list and redisplay the form
            model.Animals = await _context.Animals
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                })
                .ToListAsync();

            return View(model);
        }

        // GET: Animals/ConductCheckup/5
        public async Task<IActionResult> ConductCheckup(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            var model = new ConductCheckupViewModel
            {
                AnimalId = animal.Id,
                Animal = animal,
                HealthMetrics = new HealthMetrics(),
                TreatmentInfo = new IllnessTreatmentInfo()
            };

            return View(model);
        }

        // POST: Animals/ConductCheckup/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConductCheckup(ConductCheckupViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Fetch the animal from the database
                var animal = await _context.Animals.FindAsync(model.AnimalId);
                if (animal == null)
                {
                    return NotFound();
                }

                // Create a new MedicalRecord entry
                var medicalRecord = new MedicalRecord
                {
                    AnimalId = model.AnimalId,
                    Diagnosis = model.Diagnosis,
                    Treatment = model.TreatmentInfo.Treatment,
                    RecordDate = DateTime.Now,
                    HealthMetrics = model.HealthMetrics,
                    IllnessTreatmentInfo = model.TreatmentInfo,
                    CheckupDate = DateTime.Now
                };

                // Add the medical record to the database
                _context.MedicalRecords.Add(medicalRecord);
                await _context.SaveChangesAsync();

                // Return a JSON response for AJAX success handling
                return Json(new { success = true, message = "Diagnosis submitted successfully!" });
            }

            // Return validation errors if the model state is invalid
            return Json(new { success = false, errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
        }
    }
}
