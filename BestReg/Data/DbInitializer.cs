using BestReg.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BestReg.Data
{
    public class DbInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Apply any pending migrations and create the database if needed
            await context.Database.MigrateAsync();

            // Seed roles
            if (!roleManager.Roles.Any())
            {
                await SeedRolesAsync(roleManager);
            }

            // Seed users
            await SeedUsersAsync(userManager, roleManager);

            // Seed appointment types
            await SeedAppointmentTypesAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = new[]
            {
                "SystemAdmin",
                "FarmWorker",
                "Veterinarian",
                "FarmManager",
                "VetAdmin",
                "ExternalSupplier",
                "Admin",
                "SchoolSecurity",
                "BusDriver",
                "Student",
                "Parent"
            };

            foreach (var roleName in roles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        Console.WriteLine($"Error creating role {roleName}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }

            Console.WriteLine("Roles seeded successfully.");
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // Seed System Admin
            if (!userManager.Users.Any(u => u.UserName == "admin@gmail.com"))
            {
                await SeedUserAsync(userManager, roleManager, "admin@gmail.com", "SystemAdmin", "Admin", "User");
            }

            // Seed Farm Worker
            if (!userManager.Users.Any(u => u.UserName == "farmworker@gmail.com"))
            {
                await SeedUserAsync(userManager, roleManager, "farmworker@gmail.com", "FarmWorker", "Farm", "Worker");
            }

            // Seed Veterinarian
            if (!userManager.Users.Any(u => u.UserName == "vet@gmail.com"))
            {
                await SeedUserAsync(userManager, roleManager, "vet@gmail.com", "Veterinarian", "Veterinarian", "User");
            }

            // Seed Farm Manager
            if (!userManager.Users.Any(u => u.UserName == "farmmanager@gmail.com"))
            {
                await SeedUserAsync(userManager, roleManager, "farmmanager@gmail.com", "FarmManager", "Farm", "Manager");
            }

            // Seed Vet Admin
            if (!userManager.Users.Any(u => u.UserName == "vetadmin@gmail.com"))
            {
                await SeedUserAsync(userManager, roleManager, "vetadmin@gmail.com", "VetAdmin", "Vet", "Admin");
            }

            // Seed External Supplier
            if (!userManager.Users.Any(u => u.UserName == "externalsupplier@gmail.com"))
            {
                await SeedUserAsync(userManager, roleManager, "externalsupplier@gmail.com", "ExternalSupplier", "External", "Supplier");
            }
        }

        private static async Task SeedAppointmentTypesAsync(ApplicationDbContext context)
        {
            if (!context.AppointmentTypes.Any())
            {
                var appointmentTypes = new[]
                {
                    new AppointmentType { Name = "Check-up" },
                    new AppointmentType { Name = "Vaccination" },
                    new AppointmentType { Name = "Treatments" },
                    new AppointmentType { Name = "Nutrients" }
                };

                context.AppointmentTypes.AddRange(appointmentTypes);
                await context.SaveChangesAsync();

                Console.WriteLine("Appointment types seeded successfully.");
            }
        }

        private static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, string email, string role, string firstName, string lastName)
        {
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                IDNumber = "123456789012",
                LockoutEnabled = false,  // Ensure the account is not locked out
                EmailConfirmed = true,   // Skip email confirmation for seeded users
                QrCodeBase64 = null
            };

            var result = await userManager.CreateAsync(user, "Password@1234");
            if (result.Succeeded)
            {
                Console.WriteLine($"{firstName} {lastName} user created successfully.");

                // Check if the role exists
                if (await roleManager.RoleExistsAsync(role))
                {
                    // Assign role
                    var addToRoleResult = await userManager.AddToRoleAsync(user, role);
                    if (addToRoleResult.Succeeded)
                    {
                        Console.WriteLine($"{firstName} {lastName} user added to {role} role successfully.");
                    }
                    else
                    {
                        Console.WriteLine($"Error adding {firstName} {lastName} user to {role} role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    Console.WriteLine($"{role} role does not exist. {firstName} {lastName} user was created but not assigned to any role.");
                }
            }
            else
            {
                // Log error messages using your logging framework
                Console.WriteLine($"Error creating {firstName} {lastName} user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
