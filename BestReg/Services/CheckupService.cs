using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BestReg.Data;

namespace BestReg.Services
{
    public class CheckupService : ICheckupService
    {
        private readonly ApplicationDbContext _context;

        public CheckupService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Animal> ConductCheckupAsync(int animalId, HealthMetrics metrics, IllnessTreatmentInfo treatmentInfo)
        {
            var animal = await _context.Animals
                .Include(a => a.MedicalRecords)
                .FirstOrDefaultAsync(a => a.Id == animalId);

            if (animal == null)
            {
                return null; // Or throw an exception if preferred
            }

            // Record health metrics
            var medicalRecord = new MedicalRecord
            {
                AnimalId = animalId,
                HealthMetrics = metrics,
                IllnessTreatmentInfo = treatmentInfo,
                CheckupDate = DateTime.Now
            };

            animal.MedicalRecords.Add(medicalRecord);

            // Update the animal's overall health status, if needed
            // (This is just an example; you can add more complex logic here)
            if (treatmentInfo.IsCritical)
            {
                animal.Status = "Critical";
            }

            await _context.SaveChangesAsync();

            return animal;
        }
    }
}
