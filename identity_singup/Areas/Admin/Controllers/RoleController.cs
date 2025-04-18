using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using identity_singup.Models;
using identity_signup.Areas.Admin.Models;
using identity_signup.Extensions;
using identity_signup.Areas.Admin.Controllers;
using identity_signup.Areas.Admin.Services;
using identity_signup.Areas.Admin.ViewModels;

namespace identity_signup.Areas
{
    [Authorize(Roles = "admin, Root Admin")]
    [Area("Admin")]
    public class RoleController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IRoleService _roleService;

        public RoleController(
            RoleManager<AppRole> roleManager, 
            UserManager<AppUser> userManager,
            IRoleService roleService)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _roleService = roleService;
        }
    
        public async Task<IActionResult> Index()
        {
           
            var role = await _roleManager.Roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!
            }).ToListAsync();



            return View(role);
        }


      
        public IActionResult RoleCreate()
        {
            return View();
        }


      
        [HttpPost]
        public async Task<IActionResult> RoleCreate(RoleCreateViewModel request)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            // Root Admin kontrolü
            if (!await _roleService.IsRootAdmin(currentUser))
            {
                return RedirectToAction("AccessDenied", "Role" );
            }

            var result = await _roleManager.CreateAsync(new AppRole 
            { 
                Name = request.Name,
                PermissionLevel = request.PermissionLevel 
            });

            if (!result.Succeeded)
            {
                ModelState.AddModelErrorList(result.Errors);
                return View();
            }

            TempData["SuccessMessage"] = "Rol oluşturulmuştur.";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> RoleUpdate(string id)
        {
            var roleToUpdate = await _roleManager.FindByIdAsync(id);

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır.");
            }


            return View(new RoleUpdateViewModel() { Id = roleToUpdate.Id, Name = roleToUpdate!.Name! });
        }

      
        [HttpPost]
        public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel request)
        {

            var roleToUpdate = await _roleManager.FindByIdAsync(request.Id);

            if (roleToUpdate == null)
            {
                throw new Exception("Güncellenecek rol bulunamamıştır.");
            }

            roleToUpdate.Name = request.Name;

            await _roleManager.UpdateAsync(roleToUpdate);


            ViewData["SuccessMessage"] = "Rol bilgisi güncellenmiştir";

            return View();
        }

      
        public async Task<IActionResult> RoleDelete(string id)
        {
            var roleToDelete = await _roleManager.FindByIdAsync(id);

            if (roleToDelete == null)
            {
                throw new Exception("Silinecek rol bulunamamıştır.");
            }

            var result = await _roleManager.DeleteAsync(roleToDelete);

            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.Select(x => x.Description).First());
            }

            TempData["SuccessMessage"] = "Rol silinmiştir";
            return RedirectToAction(nameof(RoleController.Index));




        }


        public async Task<IActionResult> AssignRoleToUser(string id)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var targetUser = await _userManager.FindByIdAsync(id);
            
            if (currentUser == null || targetUser == null)
                return NotFound();
            
            ViewBag.userId = id;
            var roles = await _roleManager.Roles.ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(targetUser);
            var roleViewModelList = new List<AssignRoleToUserViewModel>();
            
            bool isCurrentUserRootAdmin = await _roleService.IsRootAdmin(currentUser);

            foreach (var roleItem in roles)
            {
                // Root Admin rolünü listeleme koşulu
                if (roleItem.Name == "Root Admin")
                    continue;
                    
                var assignRoleToUserViewModel = new AssignRoleToUserViewModel()
                {
                    Id = roleItem.Id,
                    Name = roleItem.Name!,
                    PermissionLevel = roleItem.PermissionLevel
                };

                if (userRoles.Contains(roleItem.Name!))
                {
                    assignRoleToUserViewModel.Exist = true;
                }

                roleViewModelList.Add(assignRoleToUserViewModel);
            }

            return View(roleViewModelList);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoleToUser(string userId, List<AssignRoleToUserViewModel> requestList)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var userToAssignRole = await _userManager.FindByIdAsync(userId);
            if (userToAssignRole == null)
                return NotFound();
            
            // Hedef kullanıcının Admin veya Root Admin olup olmadığını kontrol et
            bool isTargetAdmin = await _userManager.IsInRoleAsync(userToAssignRole, "admin");
            bool isTargetRootAdmin = await _userManager.IsInRoleAsync(userToAssignRole, "Root Admin");
            
            // Eğer hedef kullanıcı Admin veya Root Admin ise ve kullanıcı Root Admin değilse
            if ((isTargetAdmin || isTargetRootAdmin) && !await _roleService.IsRootAdmin(currentUser))
            {
                return RedirectToAction("AccessDenied", "Role");
            }
            
            bool anyError = false;
            bool anySuccess = false;
            bool adminRoleAttempt = false;

            foreach (var roleRequest in requestList)
            {
                var role = await _roleManager.FindByIdAsync(roleRequest.Id);
                if (role == null) continue;
                
                var userRoles = await _userManager.GetRolesAsync(userToAssignRole);
                bool userHasRole = userRoles.Contains(role.Name);
                
                // Rol durumu değişmediyse işlem yapma
                if ((userHasRole && roleRequest.Exist) || (!userHasRole && !roleRequest.Exist))
                    continue;
                
                // Admin rolü atamaya çalışıyor mu kontrol et
                if (role.Name == "admin" && roleRequest.Exist && !await _roleService.IsRootAdmin(currentUser))
                {
                    adminRoleAttempt = true;
                    continue;
                }
                
                var canModify = await _roleService.CanModifyUserRole(currentUser.Id, userId, role.Id);
                
                if (!canModify)
                {
                    anyError = true;
                    continue;
                }

                IdentityResult result;
                if (roleRequest.Exist)
                {
                    result = await _userManager.AddToRoleAsync(userToAssignRole, role.Name);
                }
                else
                {
                    result = await _userManager.RemoveFromRoleAsync(userToAssignRole, role.Name);
                }
                
                if (result.Succeeded)
                {
                    anySuccess = true;
                }
                else
                {
                    anyError = true;
                }
            }
            
            if (adminRoleAttempt)
            {
                TempData["ErrorMessage"] = "Admin rolünü atama yetkiniz bulunmamaktadır.";
            }
            else if (anyError)
            {
                TempData["ErrorMessage"] = "Bazı roller güncellenirken hata oluştu.";
            }
            
            if (anySuccess)
            {
                TempData["SuccessMessage"] = "Roller başarıyla güncellendi.";
            }
            
            return RedirectToAction(nameof(HomeController.UserList), "Home");
        }

        [Authorize(Roles = "Root Admin")]
        public async Task<IActionResult> RootAdminPanel()
        {
            var roles = await _roleService.GetAllRoles();
            var viewModel = roles.Select(x => new RoleViewModel()
            {
                Id = x.Id,
                Name = x.Name!,
                PermissionLevel = x.PermissionLevel
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Root Admin")]
        public async Task<IActionResult> UpdateRolePermission(string roleId, int permissionLevel)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Unauthorized();

            var result = await _roleService.UpdateRolePermissionLevel(roleId, permissionLevel, currentUser.Id);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Rol yetki seviyesi başarıyla güncellendi.";
            }
            else
            {
                TempData["ErrorMessage"] = "Rol yetki seviyesi güncellenirken bir hata oluştu.";
            }
            
            return RedirectToAction(nameof(RootAdminPanel));
        }

        // AccessDenied sayfası için action metodu ekleyelim
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}