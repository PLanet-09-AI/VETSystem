using System;
using System.ComponentModel.DataAnnotations;
namespace BestReg.Data

{
    public class VetAppointment
    {
        public int Id { get; set; }  // Auto-incremented primary key

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public bool IsBooked { get; set; }

        [Required]
        public string AppointmentType { get; set; } // e.g., Vaccination, Nutrition, Treatment

        [Required]
        public string VetAdminId { get; set; } // ID of the Vet Admin who created this slot

        public bool Canceled { get; set; } // Property to mark the appointment as canceled

        public string DeclineReason { get; set; }  // Field to store the reason for declining
        public bool IsDeclined { get; set; }
        public bool IsNotified { get; set; }

    }
}
