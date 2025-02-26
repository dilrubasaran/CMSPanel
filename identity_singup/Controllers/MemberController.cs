using identity_signup.ViewModels;
using identity_singup.Controllers;
using identity_singup.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

        public MemberController(ILogger<MemberController> logger, UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager, IFileProvider fileProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _fileProvider = fileProvider;
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

        [HttpGet]
        public async Task<IActionResult> UserEdit()
        {
            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);
            
            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
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
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı");
                return View(request);
            }

            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.BirthDate = request.BirthDate;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;

            if (request.Picture != null && request.Picture.Length > 0)
            {
                try 
                {
                    var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");
                    string randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(request.Picture.FileName)}";

                    var newPicturePath = Path.Combine(wwwrootFolder.First(x => x.Name == "userpictures").PhysicalPath!, randomFileName);

                    using var stream = new FileStream(newPicturePath, FileMode.Create);
                    await request.Picture.CopyToAsync(stream);

                    currentUser.Picture = randomFileName;
                }
                catch (Exception)
                {
                    ModelState.AddModelError(string.Empty, "Profil resmi yüklenirken bir hata oluştu");
                    return View(request);
                }
            }

            var updateResult = await _userManager.UpdateAsync(currentUser);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(request);
            }

            // Güvenlik damgasını güncelle ve oturumu yenile
            await _userManager.UpdateSecurityStampAsync(currentUser);
            await _signInManager.SignOutAsync();

            // Doğum tarihi varsa claim olarak ekle
            if (request.BirthDate.HasValue)
            {
                await _signInManager.SignInWithClaimsAsync(currentUser, true, 
                    new[] { new Claim("birthdate", currentUser.BirthDate.Value.ToString()) });
            }
            else
            {
                await _signInManager.SignInAsync(currentUser, true);
            }

            TempData["SuccessMessage"] = "Üye bilgileri başarıyla güncellendi.";

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName!,
                Email = currentUser.Email!,
                Phone = currentUser.PhoneNumber!,
                BirthDate = currentUser.BirthDate,
                City = currentUser.City,
                Gender = currentUser.Gender
            };

            return View(userEditViewModel);
        }

    }

}
