using System;
using System.ComponentModel.DataAnnotations;

namespace BestReg.Data
{
    public class VetAppointment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        [Required]
        [Display(Name = "Appointment Type")]
        public string AppointmentType { get; set; }

        [Required]
        public string VetAdminId { get; set; }
        public int AnimalId { get; set; } // Add this property

        public bool IsBooked { get; set; } = false;
        public bool Canceled { get; set; } = false;
        public string? DeclineReason { get; set; }
        public bool IsAccepted { get; set; }
        public bool IsDeclined { get; set; } = false;
        public bool IsNotified { get; set; } = false;
        // Navigation property to the Animal entity
      

    }
}
