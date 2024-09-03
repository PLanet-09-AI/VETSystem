using Microsoft.AspNetCore.Mvc.Rendering;

namespace BestReg.Models
{
    public class BookAppointmentViewModel
    {
        public int SelectedAnimalId { get; set; } // To hold the selected animal's ID
        public List<SelectListItem> Animals { get; set; } // To populate the dropdown

        // Other properties related to the appointment
        public string AppointmentType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
