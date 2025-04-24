using identity_signup.Extensions;
using identity_signup.Services;
using identity_signup.ViewModels;
using identity_singup.Controllers;
using identity_singup.Models;
using identity_singup.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;
using System.Security.Claims;

namespace identity_signup.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IFileProvider _fileProvider;
        private readonly UserServices _userServices;

        public MemberController(ILogger<MemberController> logger, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IFileProvider fileProvider, UserServices userServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
            _userServices = userServices;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = (await _userManager.FindByNameAsync(User.Identity!.Name!))!;

            var userViewModel = new UserViewModel
            {
                Email = currentUser.Email,
                UserName = currentUser.UserName,
                Phone = currentUser.PhoneNumber,
                PictureUrl = currentUser.Picture
            };

            return View(userViewModel);
        }


        public async Task<RedirectToActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("SignIn", "Home");

        }

        public async Task<IActionResult> UserEdit()
        {
            ViewBag.genderList = new SelectList(Enum.GetNames(typeof(Gender)));
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!)!;

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };
            return View(userEditViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {
            if (!ModelState.IsValid)
            {
                return View(request);
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            if (currentUser == null)
            {
                return View("Error");
            }

            var validationResult = await _userServices.CheckUserProfileUpdateAsync(request, currentUser);
            
            if (validationResult.IsSuccessful && !validationResult.Data)
            {
                TempData["InfoMessage"] = "Herhangi bir değişiklik yapılmadı.";
                return View(request);
            }

            if (!validationResult.IsSuccessful)
            {
                ModelState.AddModelError(string.Empty, validationResult.ErrorMessages.First());
                return View(request);
            }

            // Update işlemleri
            currentUser.UserName = request.UserName;
            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.BirthDate = request.BirthDate;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;

            var updateToUserResult = await _userManager.UpdateAsync(currentUser);
            if (!updateToUserResult.Succeeded)
            {
                ModelState.AddModelErrorList(updateToUserResult.Errors);
                return View();
            }

            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(currentUser, true);

            var claims = new List<Claim>
            {
                new Claim("PhoneNumber", currentUser.PhoneNumber ?? "")
            };
            await _signInManager.SignOutAsync();
            await _signInManager.SignInWithClaimsAsync(currentUser, true, claims);

            TempData["SuccessMessage"] = "Üye bilgileri başarıyla değiştirilmiştir";

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender,
            };

            return View(userEditViewModel);
        }

        public IActionResult AccessDenied(string ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }
    }
}
