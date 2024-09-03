namespace BestReg.Data
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public DateTime RecordDate { get; set; }
        public string Treatment { get; set; }
        public string Diagnosis { get; set; }
        public Animal Animal { get; set; }
        public IllnessTreatmentInfo IllnessTreatmentInfo { get; set; } // Property defined once
        public DateTime CheckupDate { get; set; } // Property defined once
        public HealthMetrics HealthMetrics { get; set; }

        // Additional fields if needed
    }

    public class HealthMetrics
    {
        public int Id { get; set; }
        public double Weight { get; set; }
        public double Temperature { get; set; }
        // Add other health metrics as needed
    }

    public class IllnessTreatmentInfo
    {
        public int Id { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public bool IsCritical { get; set; }
        // Add other treatment-related properties as needed
    }
}
