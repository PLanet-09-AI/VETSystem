namespace BestReg.Data
{
    public class Appointment
    {
        // Primary key, auto-incremented by the database
        public int Id { get; set; }

        // Date and time of the appointment
        public DateTime AppointmentDate { get; set; }

        // ID of the Vet Admin who scheduled the appointment
        public string VetAdminId { get; set; }

        // Status of the appointment (e.g., Scheduled, Completed, Canceled)
        public string Status { get; set; }

        // Navigation property to the related VetAdmin user
        public ApplicationUser VetAdmin { get; set; }
    }
}
