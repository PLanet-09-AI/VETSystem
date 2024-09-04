namespace BestReg.Data
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public DateTime RecordDate { get; set; } = DateTime.Now; // Automatically set the record date to now
        public string Treatment { get; set; }
        public string Diagnosis { get; set; }
        public Animal Animal { get; set; }
        public IllnessTreatmentInfo IllnessTreatmentInfo { get; set; } // Stores diagnosis and treatment info
        public HealthMetrics HealthMetrics { get; set; } // Stores health metrics like weight and temperature
        public DateTime CheckupDate { get; set; } = DateTime.Now; // Automatically set the checkup date to now

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
