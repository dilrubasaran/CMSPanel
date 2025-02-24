using identity_singup.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritaban� ba�lant�s�n� ekle
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Identity servisini ekle
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Cookie y�netimi
builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder
    {
        Name = "IdentityDeneme" // �erez ad� belirleniyor.
    };

    opt.LoginPath = new PathString("/Home/Signin"); // Kullan�c� giri� sayfas�
    opt.LogoutPath = new PathString("/Member/logout"); // ��k�� yap�l�nca y�nlendirilecek sayfa
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied"); // Yetkisiz eri�imde y�nlendirme

    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60); // �erez 60 g�n ge�erli olacak.
    opt.SlidingExpiration = true; // Kullan�c� aktif olduk�a s�re uzayacak.
});

var app = builder.Build();

// Middleware'ler
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Hatal� olan "app.MapStaticAssets()" yerine bunu ekledim.

app.UseRouting();
app.UseAuthentication(); // Kimlik do�rulama mekanizmas�n� ekledim.
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();