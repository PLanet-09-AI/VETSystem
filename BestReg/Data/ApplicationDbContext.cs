using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BestReg.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public DbSet<VetAppointment> VetAppointments { get; set; }
        public DbSet<AppointmentType> AppointmentTypes { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

       //Entities


    }
}
