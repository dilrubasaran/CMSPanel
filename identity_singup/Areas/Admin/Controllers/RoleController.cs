using identity_signup.Areas.Admin.Models;
using identity_singup.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using identity_signup.Areas.Admin.Services;

namespace identity_signup.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin, Root Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<AppRole> _roleManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleService _roleService;

        public RoleController(RoleManager<AppRole> roleManager, UserManager<AppUser> userManager, RoleService roleService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _roleService = roleService;
        }
        //INDEX
        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles
                .Where(x => x.Name != "Root Admin") // Root Admin'i listeden çıkar
                .Select(x => new RoleViewModel()
                {
                    Id = x.Id,
                    Name = x.Name!,
                    PermissionLevel = x.PermissionLevel
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
            var targetUser = await _userManager.FindByIdAsync(id);
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (targetUser == null || currentUser == null)
            {
                throw new Exception("Kullanıcı bulunamadı");
            }

            ViewBag.userId = id;
            ViewBag.UserName = targetUser.UserName;

            // Mevcut kullanıcının rollerini al
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var currentUserHighestPermission = 0;

            // En yüksek yetki seviyesini bul
            foreach (var roleName in currentUserRoles)
            {
                var userRole = await _roleManager.FindByNameAsync(roleName);
                if (userRole != null && userRole.PermissionLevel > currentUserHighestPermission)
                {
                    currentUserHighestPermission = userRole.PermissionLevel;
                }
            }

            // Hedef kullanıcının mevcut rollerini al
            var targetUserRoles = await _userManager.GetRolesAsync(targetUser);

            // Admin için özel kontrol
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "admin");
            
            // Rolleri getir
            var availableRoles = await _roleManager.Roles
                .Where(r => isAdmin ? 
                    (r.Name != "Root Admin" && r.Name != "admin") : // Admin sadece kendinden düşük rolleri görebilir
                    r.PermissionLevel < currentUserHighestPermission)
                .OrderByDescending(r => r.PermissionLevel)
                .ToListAsync();

            var roleViewModelList = new List<AssignRoleToUserViewModel>();

            foreach (var role in availableRoles)
            {
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    Id = role.Id,
                    Name = role.Name!,
                    Exist = targetUserRoles.Contains(role.Name!),
                    PermissionLevel = role.PermissionLevel
                };

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
        {
            var targetUser = await _userManager.FindByIdAsync(userId);
            var currentUser = await _userManager.GetUserAsync(User);
            
            if (targetUser == null || currentUser == null)
            {
                throw new Exception("Kullanıcı bulunamadı");
            }

            // Root Admin kontrolü
            if (await _userManager.IsInRoleAsync(targetUser, "Root Admin"))
            {
                TempData["ErrorMessage"] = "Root Admin rolü değiştirilemez";
                return RedirectToAction(nameof(HomeController.UserList), "Home");
            }

            // Admin kontrolü
            var isAdmin = await _userManager.IsInRoleAsync(currentUser, "admin");
            if (isAdmin)
            {
                // Admin, diğer adminlerin rollerini değiştiremez
                if (await _userManager.IsInRoleAsync(targetUser, "admin"))
                {
                    TempData["ErrorMessage"] = "Admin rolündeki kullanıcıların rollerini değiştiremezsiniz";
                    return RedirectToAction(nameof(HomeController.UserList), "Home");
                }
            }

            foreach (var roleViewModel in requestList)
            {
                var role = await _roleManager.FindByNameAsync(roleViewModel.Name);
                if (role == null) continue;

                // Admin için özel kontrol
                if (isAdmin && (role.Name == "Root Admin" || role.Name == "admin"))
                {
                    continue;
                }

                var hasRole = await _userManager.IsInRoleAsync(targetUser, role.Name!);

                if (roleViewModel.Exist && !hasRole)
                {
                    await _userManager.AddToRoleAsync(targetUser, role.Name!);
                }
                else if (!roleViewModel.Exist && hasRole)
                {
                    await _userManager.RemoveFromRoleAsync(targetUser, role.Name!);
                }
            }

            TempData["SuccessMessage"] = "Roller başarıyla güncellendi";
            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }

        [Authorize(Roles = "Root Admin")]
        public async Task<IActionResult> RootAdminPanel()
        {
            var roles = await _roleManager.Roles
                .Select(x => new RoleViewModel()
                {
                    Id = x.Id,
                    Name = x.Name!,
                    PermissionLevel = x.PermissionLevel
                }).ToListAsync();

            return View(roles);
        }

        [Authorize(Roles = "Root Admin")]
        public async Task<IActionResult> ManageUserRoles(string id)
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

            return View("AssignRoleToUser", roleViewModelList);
        }
    }
}