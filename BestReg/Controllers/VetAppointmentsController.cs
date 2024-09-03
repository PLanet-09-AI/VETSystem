using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using BestReg.Data;
using Microsoft.AspNetCore.Authorization;
using BestReg.Models;
using BestReg.Services;

namespace BestReg.Controllers
{
    [Authorize(Roles = "VetAdmin,Veterinarian")]
    public class VetAppointmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICheckupService _checkupService;
        private readonly IAnimalService _animalService;

        public VetAppointmentsController(ApplicationDbContext context, ICheckupService checkupService, IAnimalService animalService)
        {
            _context = context;
            _checkupService = checkupService;
            _animalService = animalService;
        }

        // GET: VetAppointments
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.VetAppointments
                .OrderBy(a => a.StartTime)
                .ToListAsync();
            return View(appointments);
        }

        // GET: VetAppointments/Create
        public IActionResult Create()
        {
            ViewBag.AppointmentTypes = new SelectList(_context.AppointmentTypes, "Name", "Name");
            return View();
        }

        // POST: VetAppointments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StartTime,EndTime,AppointmentType,VetAdminId,DeclineReason")] VetAppointment vetAppointment)
        {
            if (ModelState.IsValid)
            {
                // Set default values for the excluded properties
                vetAppointment.IsBooked = false;
                vetAppointment.Canceled = false;
                vetAppointment.IsDeclined = false;
                vetAppointment.IsNotified = false;
                vetAppointment.AnimalId = 0;
                _context.Add(vetAppointment);
                await _context.SaveChangesAsync();

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true, message = "Appointment created successfully" });
                }

                return RedirectToAction(nameof(Index));
            }

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                       .Select(e => e.ErrorMessage)
                                       .ToList();
                return Json(new { success = false, message = "Invalid data", errors = errors });
            }

            ViewBag.AppointmentTypes = new SelectList(_context.AppointmentTypes, "Name", "Name");
            return View(vetAppointment);
        }

        // GET: VetAppointments/Edit/5
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
            ViewBag.AppointmentTypes = new SelectList(_context.AppointmentTypes, "Name", "Name");
            return View(vetAppointment);
        }

        // POST: VetAppointments/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,StartTime,EndTime,AppointmentType")] VetAppointment vetAppointment)
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
            ViewBag.AppointmentTypes = new SelectList(_context.AppointmentTypes, "Name", "Name");
            return View(vetAppointment);
        }

        // GET: VetAppointments/Delete/5
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

        // POST: VetAppointments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vetAppointment = await _context.VetAppointments.FindAsync(id);
            if (vetAppointment != null)
            {
                _context.VetAppointments.Remove(vetAppointment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: VetAppointments/PendingAppointments
        public async Task<IActionResult> PendingAppointments()
        {
            var pendingAppointments = await _context.VetAppointments
                .Where(a => a.IsBooked && !a.Canceled && !a.IsDeclined && !a.IsAccepted)
                .OrderBy(a => a.StartTime)
                .ToListAsync();
            return View(pendingAppointments);
        }

        // POST: VetAppointments/AcceptAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AcceptAppointment(int id)
        {
            var appointment = await _context.VetAppointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.IsAccepted = true;
            appointment.IsNotified = false; // Set this to false so it shows up in Farm Manager's notifications
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            // TODO: Send notification to farm manager (you can implement this later)

            return RedirectToAction(nameof(PendingAppointments));
        }

        // POST: VetAppointments/DeclineAppointment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeclineAppointment(int id, string reason)
        {
            try
            {
                var appointment = await _context.VetAppointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound();
                }

                appointment.IsDeclined = true;
                appointment.DeclineReason = reason;
                appointment.IsNotified = false;
                _context.Update(appointment);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(PendingAppointments));
            }
            catch (Exception ex)
            {
                // Log the exception
                return RedirectToAction(nameof(PendingAppointments));
            }
        }

        public class DeclineAppointmentModel
        {
            public int Id { get; set; }
            public string Reason { get; set; }
        }
        private bool VetAppointmentExists(int id)
        {
            return _context.VetAppointments.Any(e => e.Id == id);
        }

        public IActionResult DiagnosisCheckup(int animalId)
        {
            var animal = _context.Animals.Find(animalId);
            if (animal == null)
            {
                return NotFound();
            }
            return View(new DiagnosisRecord { AnimalId = animalId });
        }

        [HttpPost]
        public async Task<IActionResult> DiagnosisCheckup(DiagnosisRecord record)
        {
            if (ModelState.IsValid)
            {
                record.CheckupDate = DateTime.Now;
                _context.DiagnosisRecords.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction("ScheduleVaccination", new { animalId = record.AnimalId });
            }
            return View(record);
        }


        public IActionResult ScheduleVaccination(int animalId)
        {
            var animal = _context.Animals.Find(animalId);
            if (animal == null)
            {
                return NotFound();
            }
            return View(new VaccinationSchedule { AnimalId = animalId });
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleVaccination(VaccinationSchedule schedule)
        {
            if (ModelState.IsValid)
            {
                _context.VaccinationSchedules.Add(schedule);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(schedule);
        }

        public async Task<IActionResult> RescheduleAppointment(int appointmentId)
        {
            var appointment = await _context.VetAppointments.FindAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> RescheduleAppointment(int id, DateTime newDateTime)
        {
            var appointment = await _context.VetAppointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            appointment.StartTime = newDateTime;
            appointment.EndTime = newDateTime.AddHours(1); // Assuming 1-hour duration
            _context.Update(appointment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }



        // Record Feeding Activity
        public IActionResult RecordFeeding(int animalId)
        {
            var animal = _context.Animals.Find(animalId);
            if (animal == null)
            {
                return NotFound();
            }
            return View(new FeedingPlan { AnimalId = animalId });
        }

        [HttpPost]
        public async Task<IActionResult> RecordFeeding(FeedingPlan feedingPlan)
        {
            if (ModelState.IsValid)
            {
                _context.FeedingPlans.Add(feedingPlan);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(feedingPlan);
        }

        // Request Nutrition Restock
        public IActionResult RequestRestock()
        {
            var lowStockItems = _context.NutritionStocks.Where(ns => ns.Quantity < 10).ToList();
            return View(lowStockItems);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitRestockRequest(List<int> stockIds)
        {
            // TODO: Implement logic to create restock requests for selected items
            return RedirectToAction("Index");
        }

        // Track Medical Records
        public async Task<IActionResult> MedicalRecords(int animalId)
        {
            var records = await _context.MedicalRecords
                .Where(mr => mr.AnimalId == animalId)
                .OrderByDescending(mr => mr.RecordDate)
                .ToListAsync();
            return View(records);
        }


        [HttpPost]
        public async Task<IActionResult> ScheduleEvent(Event eventModel)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Add(eventModel);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(eventModel);
        }

        public IActionResult SendFeedReminder(int id)
        {
            var animal = _context.Animals.Find(id);
            if (animal == null)
            {
                return NotFound();
            }
            return View(animal);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendFeedReminder(int id, bool confirm)
        {
            if (confirm)
            {
                // TODO: Implement email sending logic here
                TempData["Message"] = "Feed reminder sent successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> CostAnalysis()
        {
            // This is a placeholder. You'll need to implement the actual cost analysis logic.
            var totalAppointments = await _context.VetAppointments.CountAsync();
            var totalVaccinations = await _context.VaccinationSchedules.CountAsync();
            var totalFeedings = await _context.FeedingPlans.CountAsync();

            var viewModel = new CostAnalysisViewModel
            {
                TotalAppointments = totalAppointments,
                TotalVaccinations = totalVaccinations,
                TotalFeedings = totalFeedings
                // Add more data as needed
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult SearchAnimal()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchAnimal(string searchTerm)
        {
            var animals = await _context.Animals
                .Where(a => a.Name.Contains(searchTerm) || a.Id.ToString() == searchTerm)
                .ToListAsync();

            return View("SearchResults", animals);
        }
        [HttpGet]
        public async Task<IActionResult> ConductCheckup(int animalId)
        {
            if (animalId <= 0)
            {
                return BadRequest("Invalid animal ID.");
            }

            var animal = await _animalService.GetAnimalByIdAsync(animalId);
            if (animal == null)
            {
                return NotFound();
            }

            var model = new ConductCheckupViewModel
            {
                AnimalId = animalId,
                Animal = animal,
                HealthMetrics = new HealthMetrics(),
                TreatmentInfo = new IllnessTreatmentInfo()
            };

            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConductCheckup(ConductCheckupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var animal = await _checkupService.ConductCheckupAsync(model.AnimalId, model.HealthMetrics, model.TreatmentInfo);
                if (animal == null)
                {
                    return NotFound();
                }

                return RedirectToAction("ViewAnimalDetails", new { id = model.AnimalId });
            }

            return View(model);
        }

        // GET: VetAppointments/AcceptedAppointments
        public async Task<IActionResult> AcceptedAppointments()
        {
            var acceptedAppointments = await _context.VetAppointments
                .Where(a => a.IsAccepted && !a.Canceled && !a.IsDeclined)
                .OrderBy(a => a.StartTime)
                .ToListAsync();

            return View(acceptedAppointments);
        }


        //// GET: VetAppointments/DiagnosisCheckup/5
        //public async Task<IActionResult> DiagnosisCheckup(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var appointment = await _context.VetAppointments
        //        .Include(a => a.DiagnosisRecord)
        //        .FirstOrDefaultAsync(m => m.Id == id);

        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(appointment);
        //}

        //// POST: VetAppointments/DiagnosisCheckup/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DiagnosisCheckup(int id, DiagnosisRecord diagnosisRecord)
        //{
        //    var appointment = await _context.VetAppointments.FindAsync(id);

        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        appointment.DiagnosisRecord = diagnosisRecord;
        //        _context.Update(appointment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(appointment);
        //}

        //// GET: VetAppointments/ScheduleVaccination/5
        //public async Task<IActionResult> ScheduleVaccination(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var appointment = await _context.VetAppointments.FindAsync(id);

        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(appointment);
        //}

        //// POST: VetAppointments/ScheduleVaccination/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ScheduleVaccination(int id, DateTime vaccinationDate)
        //{
        //    var appointment = await _context.VetAppointments.FindAsync(id);

        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var vaccinationAppointment = new VetAppointment
        //        {
        //            StartTime = vaccinationDate,
        //            EndTime = vaccinationDate.AddHours(1), // Assuming 1-hour duration
        //            AppointmentType = "Vaccination",
        //            IsBooked = true,
        //            VetAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        //        };

        //        _context.Add(vaccinationAppointment);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(appointment);
        //}

        //// POST: VetAppointments/RescheduleAppointment/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RescheduleAppointment(int id, DateTime newDateTime)
        //{
        //    var appointment = await _context.VetAppointments.FindAsync(id);

        //    if (appointment == null)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        var duration = appointment.EndTime - appointment.StartTime;
        //        appointment.StartTime = newDateTime;
        //        appointment.EndTime = newDateTime.Add(duration);
        //        appointment.IsNotified = false; // Reset notification status

        //        _context.Update(appointment);
        //        await _context.SaveChangesAsync();

        //        // TODO: Implement logic to notify the farm manager about the rescheduled appointment

        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(appointment);
        //}
    }
}
