using BestReg.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BestReg.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("BestDBContextRegConnection") ?? throw new InvalidOperationException("Connection string 'BestDBContextRegConnection' not found.");

builder.Services.AddDbContext<BestDBContextReg>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<BestRegUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<BestDBContextReg>();

// Add services to the container
builder.Services.AddControllersWithViews();

var app = builder.Build();

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
