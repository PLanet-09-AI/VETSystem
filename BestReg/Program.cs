using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BestReg.Areas.Identity.Data;

var builder = WebApplication.CreateBuilder(args);

// Retrieve the connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("BestDBContextRegConnection")
    ?? throw new InvalidOperationException("Connection string 'BestDBContextRegConnection' not found.");

// Configure DbContext with retry logic
builder.Services.AddDbContext<BestDBContextReg>(options =>
    options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// Configure Identity services
builder.Services.AddDefaultIdentity<BestRegUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<BestDBContextReg>();

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
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
