using System;
using System.ComponentModel.DataAnnotations;

namespace BestReg.Models
{
    public class CreateVetAppointmentViewModel
    {
        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }

        [Required]
        [Display(Name = "Appointment Type")]
        public string AppointmentType { get; set; }
    }
}
