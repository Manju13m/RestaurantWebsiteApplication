using Azure.Core;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using RestaurantWebsiteApplication.Data;
using RestaurantWebsiteApplication.email;
using RestaurantWebsiteApplication.excel;
using RestaurantWebsiteApplication.Password;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Inject dbcontext into application
builder.Services.AddDbContext<RestaurantDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("RestrauntDbConnectionString"))
);

// Add IHttpContextAccessor
builder.Services.AddHttpContextAccessor();


// Configure cookie-based authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Log"; // Set the login path
        options.AccessDeniedPath = "/Login/AccessDenied"; // Set the access denied path
    });

// Add session services
builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP-only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

//Add email service
builder.Services.AddScoped<IEmailService, EmailService>();


//Add ExcelReportGenerator service
builder.Services.AddTransient<IExcelReportGenerator, ExcelReportGenerator>();

// Register PasswordService as scoped service
builder.Services.AddScoped<PasswordService>();

// Set EPPlus LicenseContext to NonCommercial
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

// Configure request localization options
/*var supportedCultures = new[]
{
    new CultureInfo("en-GB") // English (United Kingdom) culture
};*/


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Enable authentication and authorization
app.UseAuthentication();

app.UseAuthorization();

// Enable session middleware
app.UseSession();


// Configure request localization
/*var requestLocalizationOptions = new RequestLocalizationOptions
                                 {
                                     DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en-GB"),
                                     SupportedCultures = supportedCultures,
                                     SupportedUICultures = supportedCultures
                                 };

app.UseRequestLocalization(requestLocalizationOptions);*/

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
