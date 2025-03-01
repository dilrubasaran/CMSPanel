using identity_singup.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritabanı bağlantısını ekle
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // Şifre gereksinimleri
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;

    // Kullanıcı gereksinimleri
    options.User.RequireUniqueEmail = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// IFileProvider servisi
builder.Services.AddSingleton<IFileProvider>(new PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")));

// Cookie yönetimi
builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder
    {
        Name = "IdentityDeneme" // Çerez adı belirleniyor.
    };

    opt.LoginPath = new PathString("/Home/Signin"); // Kullanıcı giriş sayfası
    opt.LogoutPath = new PathString("/Member/logout"); // Çıkış yapılınca yönlendirilecek sayfa
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied"); // Yetkisiz erişimde yönlendirme

    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60); // Çerez 60 gün geçerli olacak.
    opt.SlidingExpiration = true; // Kullanıcı aktif oldukça süre uzayacak.
});

builder.Services.AddScoped<IEducationServices, EducationService>();

var app = builder.Build();

// Rolleri otomatik oluştur
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

    var roles = new[] { "admin", "egitimci", "ogrenci" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new AppRole { Name = role });
        }
    }
}

// wwwroot/userpictures klasörünü oluştur
var picturesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "userpictures");
if (!Directory.Exists(picturesDirectory))
{
    Directory.CreateDirectory(picturesDirectory);
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

app.UseHttpsRedirection();
app.UseStaticFiles(); // Hatalı olan "app.MapStaticAssets()" yerine bunu ekledim.

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