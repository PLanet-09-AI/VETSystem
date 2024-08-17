using BestReg.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<BestRegUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Define roles to be created
        string[] roleNames = { "Admin", "Parent", "SchoolAuthority", "BusDriver" };

        // Ensure roles are created
        await EnsureRoles(roleManager, roleNames);

        // Seed the admin user
        var adminEmail = "admin@school.com";
        var adminPassword = "SuperSecurePassword12345!@#$%^&*()"; // Replace with your secure password

        // Ensure Admin user is created and assigned the Admin role
        await EnsureUser(userManager, roleManager, adminEmail, adminPassword, "Admin", "User", "DefaultProvince", new string[] { "Admin" });
    }

    public static async Task EnsureRoles(RoleManager<IdentityRole> roleManager, string[] roleNames)
    {
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                {
                    throw new Exception($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }

    public static async Task EnsureUser(UserManager<BestRegUser> userManager, RoleManager<IdentityRole> roleManager, string email, string password, string firstName, string lastName, string province, string[] roles)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new BestRegUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                Province = province,
                SourceOfFunding = "DefaultSource", // Example default value for SourceOfFunding
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                foreach (var role in roles)
                {
                    if (!await userManager.IsInRoleAsync(user, role))
                    {
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }
            else
            {
                throw new Exception($"Failed to create user '{email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
        else
        {
            // Ensure the user is in the required roles
            foreach (var role in roles)
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}
