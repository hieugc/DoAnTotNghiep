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
using DoAnTotNghiep.Hubs;
using Hangfire;
using DoAnTotNghiep.Modules;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<DoAnTotNghiepContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("DataContext")));
builder.Services.AddSignalR();


builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(Scheme.Authentication())
    .AddJwtBearer(Scheme.AuthenticationJWT(), options =>
    {
        options.RefreshInterval.Add(TimeSpan.FromMinutes(15));
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration.GetConnectionString(ConfigurationJWT.JwtBearerIssuer()),
            ValidAudience = builder.Configuration.GetConnectionString(ConfigurationJWT.JwtBearerAudience()),
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
        options.ReturnUrlParameter = new PathString("/Home/Index");
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
            string? url = context.Request.Path.Value;
            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer") || !string.IsNullOrEmpty(url) && url.Contains("api"))
                return Scheme.AuthenticationJWT();

            // otherwise always check for cookie auth
            return Scheme.AuthenticationCookie();
        };
    });

//builder.Services.AddHostedService<TimedHostedService>();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/UnderMaintenance");
}

app.Use(async (context, next) =>
{
    if (!context.Request.Path.ToString().ToLower().Contains("api"))
    {
        if (context.Response.StatusCode >= 500)
        {
            context.Request.Path = "/Home/UnderMaintenance";
        }
        else if (context.Response.StatusCode >= 400)
        {
            context.Request.Path = "/Home/NotFound";
            await next();
        }
    }
    await next();
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/ChatHub");

app.Run();
