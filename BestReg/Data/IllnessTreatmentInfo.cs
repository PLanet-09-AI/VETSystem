namespace BestReg.Data
{
    public class IllnessTreatmentInfo
    {
        public int Id { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public bool IsCritical { get; set; }
        // Add other treatment-related properties as needed
    }
}
