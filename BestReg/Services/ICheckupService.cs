using System.Threading.Tasks;
using BestReg.Data;

namespace BestReg.Services
{
    public interface ICheckupService
    {
        Task<Animal> ConductCheckupAsync(int animalId, HealthMetrics metrics, IllnessTreatmentInfo treatmentInfo);
    }
}
