namespace BestReg.Data
{
    public class Animal
    {
        public int Id { get; set; }
        public string Species { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<DiagnosisRecord> DiagnosisRecords { get; set; }
        public List<VaccinationSchedule> VaccinationSchedules { get; set; }
        public List<FeedingPlan> FeedingPlans { get; set; }
        public List<MedicalRecord> MedicalRecords { get; set; }
    }
}
