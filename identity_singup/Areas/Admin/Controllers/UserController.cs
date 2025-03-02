using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using identity_singup.Models;
using Microsoft.EntityFrameworkCore;

namespace identity_singup.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public UserController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // Kullanıcı listesi sayfası
        public async Task<IActionResult> UserStatus()
        {
            var users = await _userManager.Users.ToListAsync();
            return View(users);
        }


        // Kullanıcı aktiflik durumunu değiştirme
        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            user.IsActive = !user.IsActive;
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = $"Kullanıcı durumu {(user.IsActive ? "aktif" : "pasif")} olarak güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Kullanıcı durumu güncellenirken bir hata oluştu.";
            }

            return RedirectToAction(nameof(UserStatus));
        }
    }
} 