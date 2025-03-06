using identity_singup.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System.Security;
using identity_signup.Areas.Instructor.Services;
using identity_singup.Areas.Admin.Services;
using identity_singup.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using Identity.Infrastructure.Authorization;
using identity_singup.Infrastructure;
using Microsoft.AspNetCore.HttpOverrides;
using identity_signup.Areas.Admin.Services;
using identity_singup.Areas.Admin.Repositories;
using identity_singup.Areas.Admin.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritabanı bağlantısını ekle
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//? Testdb için olan veritabanı eklentisi
//var isTesting = Environment.GetEnvironmentVariable("USE_TEST_DB") == "true";
//Console.WriteLine($"Using TestDB: {isTesting}");

//var connectionString = builder.Configuration.GetConnectionString(isTesting ? "TestConnection" : "DefaultConnection");
//Console.WriteLine($"Bağlanılan veritabanı: {connectionString}");

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlServer(connectionString));

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
    opt.ExpireTimeSpan = TimeSpan.FromDays(60); // Cookie 60 gün geçerli olacak.
    opt.SlidingExpiration = true; // Kullanıcı aktif oldukça süre uzayacak.
});

// Proxy ve forwarded headers ayarları
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

// Diğer servislerden önce
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

builder.Services.AddScoped<IRoleService, RoleService>();

// Menü ve Claims servislerini kaydet
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IClaimsService, ClaimsService>();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Veritabanı ve seed işlemleri için scope oluştur
using (var scope = app.Services.CreateScope())
{
   
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        // Veritabanını oluştur
       
        await context.Database.EnsureCreatedAsync();
    
        await MenuSeedData.SeedMenuItemsAsync(app.Services);
        
        // Rolleri ve admin kullanıcısını oluştur
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

        // Rolleri oluştur
        var roles = new[]
        {
            new { Name = "Root Admin", Level = 100 },
            new { Name = "Admin", Level = 50 },
            new { Name = "Instructor", Level = 10 },
            new { Name = "Student", Level = 1 }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                await roleManager.CreateAsync(new AppRole 
                { 
                    Name = role.Name, 
                    PermissionLevel = role.Level 
                });
            }
        }

        // Root Admin kullanıcısını oluştur
        var rootAdminUsername = "rootadmin";
        var rootAdminEmail = "rootadmin@gmail.com";
        var rootAdmin = await userManager.FindByNameAsync(rootAdminUsername);

        if (rootAdmin == null)
        {
            rootAdmin = new AppUser
            {
                UserName = rootAdminUsername,
                Email = rootAdminEmail,
                EmailConfirmed = true,
                IsRootAdmin = true
            };

            var result = await userManager.CreateAsync(rootAdmin, "Password12*"); 
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(rootAdmin, "Root Admin");
            }
        }
    
   
}

// Middleware'ler
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Geliştirme ortamında detaylı hata sayfası
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseForwardedHeaders(); // En üstte olmalı
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