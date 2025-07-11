using System.Diagnostics;
using System.Security.Claims;
using identity_signup.ViewModels;
using Microsoft.AspNetCore.Mvc;
using identity_singup.Models;
using identity_singup.ViewModels;
using Microsoft.AspNetCore.Identity;
using identity_signup.Services;



namespace identity_singup.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly  IUserService _userService;




    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager,IUserService  userService)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _userService = userService;
    }

    public IActionResult Index()
    {
        return View();
    }



    public IActionResult SignUp()
    {
        return View();
    }

    [HttpPost]
    public async  Task<IActionResult> SignUp(SignUpViewModel request)
    {
        if (!ModelState.IsValid)
        {
            return View();
        }

        var validationResult = await _userService.ValidateUserAsync<SignUpViewModel>(request);
        if (!validationResult.IsSuccessful)
        {
            ModelState.AddModelError(string.Empty, validationResult.ErrorMessages.First());
            return View(request);
        }

        var identityResult = await _userManager.CreateAsync(new() 
        { 
            UserName = request.UserName, 
            Email = request.Email, 
            PhoneNumber = request.Phone
        }, request.PasswordConfirm);

        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        TempData["SuccessMessage"] = "Üyelik kaydınız başarılı bir şekilde oluşturulmuştur.";
        return RedirectToAction(nameof(HomeController.Index));
    }

    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]

    public async Task<IActionResult> SignIn(SignInViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userManager.FindByNameAsync(model.UserName);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Sistemde kayıtlı kullanıcı bulunamadı, önce kayıt olmalısınız");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, true);
        if (result.Succeeded)
        {
            var claims = new List<Claim>();

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                claims.Add(new Claim("PhoneNumber", user.PhoneNumber));
            }

            // Önce mevcut claims'leri temizle
            await _signInManager.SignOutAsync();
            
            // Yeni claims'lerle sign in yap
            await _signInManager.SignInWithClaimsAsync(user, model.RememberMe, claims);

            // Kullanıcının rollerini al
            var roles = await _userManager.GetRolesAsync(user);
            
            // Rol bazlı yönlendirme
            if (roles.Contains("Admin"))
            {
                return RedirectToAction("Dashboard", "Home", new { area = "Admin" });
            }
            else if (roles.Contains("Instructor"))
            {
                return RedirectToAction("Dashboard", "Home", new{area = "Instructor"});
            }
            else if (roles.Contains("Student"))
            {
                return RedirectToAction("Dashboard", "Home", new{area = "Student"});
            }

            // Eğer hiçbir rol bulunamazsa ana sayfaya yönlendir
            return RedirectToAction("Index", "Home");
        }


        if (result.IsNotAllowed)
        {
            ModelState.AddModelError(string.Empty, "Hesabınız pasif durumdadır. Lütfen yönetici ile iletişime geçin.");
            return View(model);
        }


        ModelState.AddModelError(string.Empty, "Kullanıcı adı veya şifre yanlış");
        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

   
}
