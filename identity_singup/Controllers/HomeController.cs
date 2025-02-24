using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using identity_singup.Models;
using identity_singup.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace identity_singup.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly UserManager<AppUser> _userManager;
    //private readonly SingInManager<AppUser> _singInManager;

    public HomeController(ILogger<HomeController> logger, UserManager<AppUser> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
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

        var identityResult = await _userManager.CreateAsync(new() { UserName = request.UserName, Email= request.Email, PhoneNumber= request.Phone,}, request.PasswordConfirm);

        if (!identityResult.Succeeded)
        {
            foreach (var error in identityResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
            return View();
        }

        TempData["Succeeded Message"] = "Üyelik kaydýnýz baþarýlý bir þekilde oluþturulmuþtur.";

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }










    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
