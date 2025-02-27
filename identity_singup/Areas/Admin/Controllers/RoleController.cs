using identity_signup.Areas.Admin.Models;
using identity_singup.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace identity_signup.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;

        public RoleController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        //INDEX
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();

            return View(roles);
        }

        //ROLECREATE
        public IActionResult RoleCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = request.Name });

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Rol oluşturulurken bir hata oluştu");
                return View();
            }

            TempData["SuccessMessage"] = "Rol oluşturuldu";
            return RedirectToAction(nameof(RoleController.Index));
        }

        //ROLEUPDATE
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamadı");
            }

            return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id, Name = roleToUpdate.Name! });
        }

        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamadı");
            }

            roleToUpdate.Name = request.Name;
            await _roleManager.UpdateAsync(roleToUpdate);

            TempData["SuccessMessage"] = "Rol güncellendi";
            return RedirectToAction(nameof(RoleController.Index));
        }

        //ROLEDELETE
        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);

            if (roleToDelete == null)
            {
                throw new Exception("Silinecek rol bulunamadı");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                throw new Exception("Rol silinemedi");
            }

            TempData["SuccessMessage"] = "Rol silindi";
            return RedirectToAction(nameof(RoleController.Index));
        }

        //ASSIGNROLETTOUSER

        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = await _userManager.FindByIdAsync(id);
            
            if (currentUser == null)
            {
                throw new Exception("Kullanıcı bulunamadı");
            }

            ViewBag.userId = id;

            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(currentUser);
            var roleViewModelList = new List<AssignRoleToUserViewModel>();

            foreach (var role in roles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    Id = role.Id,
                    Name = role.Name!,
                    Exist = userRoles.Contains(role.Name!)
                };

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
        {
            var userToAssignRoles = await _userManager.FindByIdAsync(userId);
            
            if (userToAssignRoles == null)
            {
                throw new Exception("Kullanıcı bulunamadı");
            }

            foreach (var role in requestList)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role.Name);
                if (!roleExists)
                {
                    continue;
                }

                var hasRole = await _userManager.IsInRoleAsync(userToAssignRoles, role.Name);

                if (role.Exist && !hasRole)
                {
                    await _userManager.AddToRoleAsync(userToAssignRoles, role.Name);
                }
                else if (!role.Exist && hasRole)
                {
                    await _userManager.RemoveFromRoleAsync(userToAssignRoles, role.Name);
                }
            }

            TempData["SuccessMessage"] = "Roller başarıyla güncellendi";
            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }
    }
}