using BestReg.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<BestRegUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // Define roles to be created
        string[] roleNames = { "Admin", "Parent", "SchoolAuthority", "BusDriver" };

        // Seed roles
        await EnsureRoles(roleManager, roleNames);

        // Seed the admin user
        var adminEmail = "admin@school.com";
        var adminPassword = "SuperSecurePassword12345!@#$%^&*()";
        await EnsureUser(userManager, adminEmail, adminPassword, "Admin", "User", "DefaultProvince", new string[] { "Admin" });
    }

    // Make this method public so it can be accessed from other classes
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

    // Make this method public if needed for access from other classes
    public static async Task EnsureUser(UserManager<BestRegUser> userManager, string email, string password, string firstName, string lastName, string province, string[] roles)
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
                    await userManager.AddToRoleAsync(user, role);
                }
            }
            else
            {
                throw new Exception($"Failed to create user '{email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}
