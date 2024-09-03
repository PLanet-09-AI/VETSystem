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
        public IllnessTreatmentInfo IllnessTreatmentInfo { get; set; } // Add this property
        public DateTime CheckupDate { get; set; } // Add this property

        // Additional fields if needed

        public IllnessTreatmentInfo IllnessTreatmentInfo { get; set; }
        public DateTime CheckupDate { get; set; }


        public HealthMetrics HealthMetrics { get; set; }

    }
    public class HealthMetrics
    {
        public int Id { get;set;}
        public double Weight { get; set; }
        public double Temperature { get; set; }
        // Add other health metrics as needed
    }

    public class IllnessTreatmentInfo
    {
        public int 
            Id
        { get;set;}
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        // Add other treatment-related properties as needed
    }
}
