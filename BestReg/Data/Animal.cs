namespace BestReg.Data
{
    public class Animal
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string Species { get; set; }
        public DateTime DateOfBirth { get; set; }
        // Initialize these collections in the constructor
        public string Status { get; set; } // Add this property
        public List<DiagnosisRecord> DiagnosisRecords { get; set; } = new List<DiagnosisRecord>();
        public List<VaccinationSchedule> VaccinationSchedules { get; set; } = new List<VaccinationSchedule>();
        public List<FeedingPlan> FeedingPlans { get; set; } = new List<FeedingPlan>();
        public List<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
    }


}