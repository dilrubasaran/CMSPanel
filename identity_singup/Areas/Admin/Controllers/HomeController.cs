using identity_signup.Areas.Admin.Models;
using identity_signup.ViewModels;
using identity_singup.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace identity_signup.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,Root Admin")]
    public class HomeController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public HomeController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> UserList()
        {
            var users = await _userManager.Users.ToListAsync();
            var userViewModels = new List<UserListViewModel>();

            foreach (var user in users)
            {
                var userViewModel = new UserListViewModel
                {
                    Id = user.Id,
                    Name = user.UserName!,
                    Email = user.Email!
                };
                userViewModels.Add(userViewModel);
            }

            return View(userViewModels);
        }
    }
}
