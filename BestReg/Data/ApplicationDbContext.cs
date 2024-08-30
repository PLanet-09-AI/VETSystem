using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BestReg.Data
{
    public class ApplicationDbContext:IdentityDbContext<ApplicationUser>
    {
        public DbSet<VetAppointment> VetAppointments { get; set; }
        public DbSet<AppointmentType> AppointmentTypes { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        // New DbSets
        public DbSet<DiagnosisRecord> DiagnosisRecords { get; set; }
        public DbSet<VaccinationSchedule> VaccinationSchedules { get; set; }
        public DbSet<FeedingPlan> FeedingPlans { get; set; }
        public DbSet<NutritionStock> NutritionStocks { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Event> Events { get; set; }

        // If you have an Animal model, add it as well
        public DbSet<Animal> Animals { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

       //Entities


    }
}
