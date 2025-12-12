using System.Globalization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Localization;
using TourAgencyClient.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddNewtonsoftJson();

builder.Services.AddHttpClient("UserApi", c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServiceUrls:User"]!));
builder.Services.AddHttpClient("TourApi", c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServiceUrls:Tour"]!));
builder.Services.AddHttpClient("BookingApi", c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServiceUrls:Booking"]!));
builder.Services.AddHttpClient("PaymentApi", c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServiceUrls:Payment"]!));
builder.Services.AddHttpClient("PartnerApi", c =>
    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:ServiceUrls:Partner"]!));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(3);
    });

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<ITourService, TourService>();
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IUserService, UserService>();


var defaultCulture = "ru-RU";
var localizationOptions = new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture(defaultCulture),
    SupportedCultures = new List<CultureInfo> { new CultureInfo(defaultCulture) },
    SupportedUICultures = new List<CultureInfo> { new CultureInfo(defaultCulture) }
};

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRequestLocalization(localizationOptions);
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
