namespace BestReg.Data
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string VetAdminId { get; set; }
        public string Status { get; set; }

        // Relationships
        public ApplicationUser VetAdmin { get; set; }
    }
}
