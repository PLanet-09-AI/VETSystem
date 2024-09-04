using BestReg.Data;
using System.Collections.Generic;

namespace BestReg.Models
{
    public class AddAnimalToAppointmentViewModel
    {
        public int AppointmentId { get; set; }
        public int SelectedAnimalId { get; set; }
        public List<Animal> Animals { get; set; }
    }
}
