using DoAnTotNghiep.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
#pragma warning disable CS8604 // Possible null reference argument.
builder.Services.AddDbContext<DoAnTotNghiepContext>(options =>
    options.UseSqlServer(connectionString: builder.Configuration.GetConnectionString("DataContext")));
#pragma warning restore CS8604 // Possible null reference argument.

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

app.Run();
