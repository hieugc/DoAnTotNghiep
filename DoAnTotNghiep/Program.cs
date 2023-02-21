using DoAnTotNghiep.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DoAnTotNghiepContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext") ?? throw new InvalidOperationException("Connection string 'DoAnTotNghiep' not found.")));


builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication("SecurityScheme")
    .AddCookie("SecurityScheme", options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
        options.LoginPath = new PathString("/Auth/Login");
        options.LogoutPath = new PathString("/Auth/Logout/");
        options.AccessDeniedPath = new PathString("/Auth/Login");
        options.ReturnUrlParameter = "/Home/Index";
        options.SlidingExpiration = true;
        options.Cookie = new CookieBuilder
        {
            //Domain = "",
            HttpOnly = true,
            Name = "Security.Cookie",
            Path = "/",
            SameSite = SameSiteMode.Lax,
            SecurePolicy = CookieSecurePolicy.SameAsRequest
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
