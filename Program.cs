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

// Identity servisini AppUser ile düzelt
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// IFileProvider servisini ekle
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

var app = builder.Build();

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
app.UseAuthentication(); // Kimlik doğrulama mekanizmasını ekledim.
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();