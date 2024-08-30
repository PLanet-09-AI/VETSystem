namespace BestReg.Data
{
    public class VaccinationSchedule
    {
        public int Id { get; set; }
        public int AnimalId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public string VaccineType { get; set; }
        public Animal Animal { get; set; }
    }
}
