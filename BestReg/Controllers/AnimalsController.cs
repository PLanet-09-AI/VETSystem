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
using BestReg.Services;

namespace BestReg.Controllers
{
    [Authorize(Roles = "FarmWorker,Veterinarian")]
    public class AnimalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnimalsController> _logger;
        private readonly ICheckupService _checkupService;

        public AnimalsController(ApplicationDbContext context, ILogger<AnimalsController> logger, ICheckupService checkupService)
        {
            _context = context;
            _logger = logger;
            _checkupService = checkupService;
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
                _logger.LogInformation("Animal created successfully with ID: {AnimalId}", animal.Id);
                return RedirectToAction(nameof(Index));
            }

            _logger.LogInformation("ModelState is invalid.");
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

        // Action for vets to conduct a checkup
        [HttpGet]
        public async Task<IActionResult> ConductCheckup(int animalId)
        {
            var animal = await _context.Animals.FindAsync(animalId);
            if (animal == null)
            {
                return NotFound();
            }

            var viewModel = new ConductCheckupViewModel
            {
                AnimalId = animal.Id,
                Name = animal.Name,
                Status = animal.Status,
                Species = animal.Species,
                HealthMetrics = new HealthMetrics(),
                TreatmentInfo = new IllnessTreatmentInfo(),
                Animal = animal
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ConductCheckup(ConductCheckupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var animal = await _context.Animals.FindAsync(model.AnimalId);
                if (animal != null)
                {
                    // Create a new MedicalRecord entry
                    var medicalRecord = new MedicalRecord
                    {
                        AnimalId = model.AnimalId,
                        Diagnosis = model.TreatmentInfo.Diagnosis,
                        Treatment = model.TreatmentInfo.Treatment,
                        HealthMetrics = model.HealthMetrics,
                        IllnessTreatmentInfo = model.TreatmentInfo,
                        CheckupDate = DateTime.Now // Set the checkup date to the current date
                    };

                    // Add the medical record to the animal's medical records
                    animal.MedicalRecords.Add(medicalRecord);

                    // Save the changes to the database
                    _context.MedicalRecords.Add(medicalRecord);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("AcceptedAppointments");
                }
            }

            return View(model);
        }
        
        // Action for farmworkers to add an animal to an appointment
        [HttpGet]
        public IActionResult AddAnimalToAppointment(int appointmentId)
        {
            var viewModel = new AddAnimalToAppointmentViewModel
            {
                AppointmentId = appointmentId,
                Animals = _context.Animals.ToList() // Fetch all animals from the database
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddAnimalToAppointment(AddAnimalToAppointmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var appointment = await _context.VetAppointments.FindAsync(model.AppointmentId);
                if (appointment != null)
                {
                    appointment.AnimalId = model.SelectedAnimalId;
                    await _context.SaveChangesAsync();
                    return RedirectToAction("AcceptedAppointments");
                }
            }

            return View(model);
        }


        // GET: Animals/SearchAnimal
        [HttpGet]
        public IActionResult SearchAnimal()
        {
            return View();
        }

        // POST: Animals/SearchAnimal
        [HttpPost]
        public async Task<IActionResult> SearchAnimal(string searchTerm)
        {
            var animals = await _context.Animals
                .Where(a => a.Name.Contains(searchTerm) || a.Id.ToString() == searchTerm)
                .ToListAsync();

            return View("SearchResults", animals);
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
                await _context.SaveChangesAsync();
            }

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
    }
}
