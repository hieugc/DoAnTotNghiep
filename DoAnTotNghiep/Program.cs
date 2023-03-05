using DoAnTotNghiep.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using DoAnTotNghiep.Enum;
using NuGet.Protocol;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);


//string? StringConnectSQL = builder.Configuration.GetConnectionString("DataContext");
//if(StringConnectSQL == null)
//{
//    Console.WriteLine("null");
//    StringConnectSQL = "Data Source=(localdb)\\local;Initial Catalog=DoAnTotNghiep;Persist Security Info=True;User ID=doantotnghiep;Password=12345678;MultipleActiveResultSets=True";
//}

// Add services to the container.
builder.Services.AddDbContext<DoAnTotNghiepContext>(options => 
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DataContext") 
        ?? throw new InvalidOperationException("Connection string 'DoAnTotNghiep' not found.")
    )
);


builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(Scheme.Authentication())
    .AddJwtBearer(Scheme.AuthenticationJWT(), options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration.GetConnectionString(ConfigurationJWT.JwtBearerIssuer()),
            ValidateAudience = true,
            ValidAudience = builder.Configuration.GetConnectionString(ConfigurationJWT.JwtBearerAudience()),
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetConnectionString(ConfigurationJWT.JwtBearerIssuerSigningKey()) 
                                ?? throw new InvalidOperationException("JwtBearerIssuerSigningKey NOT FOUND")))
        };
    })
    .AddCookie(Scheme.AuthenticationCookie(), options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromSeconds(10);
        options.LoginPath = new PathString("/Authorize/SignIn");
        options.LogoutPath = new PathString("/Authorize/Logout/");
        options.AccessDeniedPath = new PathString("/Authorize/SignIn");
        options.ReturnUrlParameter = "/Home/Index";
        options.SlidingExpiration = true;
        options.Cookie = new CookieBuilder
        {
            //Domain = "",
            HttpOnly = true,
            Name = Scheme.AuthenticationCookie(),
            Path = "/",
            SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax,
            SecurePolicy = CookieSecurePolicy.SameAsRequest
        };
    })// this is the key piece!
    .AddPolicyScheme(Scheme.Authentication(), Scheme.Authentication(), options =>
    {
        // runs on each request
        options.ForwardDefaultSelector = context =>
        {
            // filter by auth type
            string? authorization = context.Request.Headers[HeaderNames.Authorization];
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer"))
                return Scheme.AuthenticationJWT();

            // otherwise always check for cookie auth
            return Scheme.AuthenticationCookie();
        };
    });

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/NotFound");
}

app.Use(async (context, next) =>
{
    await next();
    if (!context.Request.Path.ToString().Contains("api"))
    {
        if (context.Response.StatusCode >= 400)
        {
            context.Request.Path = "/Home/NotFound";
            await next();
        }
    }
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
