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

    }
}
