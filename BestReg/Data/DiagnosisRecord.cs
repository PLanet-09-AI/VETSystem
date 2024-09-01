namespace BestReg.Data
{
    public class DiagnosisRecord
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public DateTime CheckupDate { get; set; }
        public int Age { get; set; }
        public float Height { get; set; }
        public float Weight { get; set; }
        public string IllnessStatus { get; set; }
        public string TreatmentStatus { get; set; }

        // Navigation property
        public Animal Animal { get; set; }
    }
}
