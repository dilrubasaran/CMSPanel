using identity_singup.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Security;
using identity_signup.Areas.Instructor.Services;
using identity_signup.Infrastructure;
using identity_singup.Areas.Admin.Services;
using identity_singup.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Identity.Infrastructure.Authorization;
using identity_singup.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritabanı bağlantısını ekle
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Identity servisini ekle
builder.Services.AddIdentity<AppUser, AppRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
// IFileProvider servisini ekle
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));
builder.Services.AddScoped<IEducationServices, EducationService>();

builder.Services.AddScoped<IEducationServices, EducationService>();
builder.Services.AddScoped<IPermissionRequestService, PermissionRequestService>();
builder.Services.AddScoped<SignInManager<AppUser>, CustomSignInManager>();

// Authorization policy'sini ekle
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanEditEducation", policy =>
        policy.AddRequirements(new CanEditEducationPolicy(1))); // 1 dakika
});

// Authorization handler'ı ekle
builder.Services.AddScoped<IAuthorizationHandler, EducationAuthorizationHandler>();
// Cookie yönetimi
builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder
    {
        Name = "IdentityDeneme"
    };

    opt.LoginPath = new PathString("/Home/Signin"); 
    opt.LogoutPath = new PathString("/Member/logout"); 
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied"); 

    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60); // �erez 60 g�n ge�erli olacak.
    opt.SlidingExpiration = true; // Kullanıcı aktif oldukça s�re uzayacak.
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

}

// Middleware'ler
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Geli�tirme ortam�nda detayl� hata sayfas�
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); 

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


  


app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();