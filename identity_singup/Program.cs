using identity_singup.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Veritabaný baðlantýsýný ekle
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Identity servisini ekle
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Cookie yönetimi
builder.Services.ConfigureApplicationCookie(opt =>
{
    var cookieBuilder = new CookieBuilder
    {
        Name = "IdentityDeneme" // Çerez adý belirleniyor.
    };

    opt.LoginPath = new PathString("/Home/Signin"); // Kullanýcý giriþ sayfasý
    opt.LogoutPath = new PathString("/Member/logout"); // Çýkýþ yapýlýnca yönlendirilecek sayfa
    opt.AccessDeniedPath = new PathString("/Member/AccessDenied"); // Yetkisiz eriþimde yönlendirme

    opt.Cookie = cookieBuilder;
    opt.ExpireTimeSpan = TimeSpan.FromDays(60); // Çerez 60 gün geçerli olacak.
    opt.SlidingExpiration = true; // Kullanýcý aktif oldukça süre uzayacak.
});

var app = builder.Build();

// Middleware'ler
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(); // Hatalý olan "app.MapStaticAssets()" yerine bunu ekledim.

app.UseRouting();
app.UseAuthentication(); // Kimlik doðrulama mekanizmasýný ekledim.
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();