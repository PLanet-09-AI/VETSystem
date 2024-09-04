using BestReg.Data;
using System.ComponentModel.DataAnnotations;

namespace BestReg.Models
{
    public class ConductCheckupViewModel
    {
        public int AnimalId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public string Species { get; set; }
        public string Diagnosis { get; set; }
        public HealthMetrics HealthMetrics { get; set; }
        public IllnessTreatmentInfo TreatmentInfo { get; set; }
        public Animal Animal { get; set; }
    }
}
