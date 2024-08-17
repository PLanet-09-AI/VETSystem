using BestReg.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

var builder = WebApplication.CreateBuilder(args);

// Setup database connection string
var connectionString = builder.Configuration.GetConnectionString("BestDBContextRegConnection")
    ?? throw new InvalidOperationException("Connection string 'BestDBContextRegConnection' not found.");

// Configure services for DbContext and Identity
builder.Services.AddDbContext<BestDBContextReg>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<BestRegUser>(options =>
    options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()  // Add roles to the Identity system
    .AddEntityFrameworkStores<BestDBContextReg>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Seed roles and admin user on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<BestRegUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SeedData.Initialize(services, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
