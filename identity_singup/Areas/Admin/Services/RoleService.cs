using identity_singup.Models;
using Microsoft.AspNetCore.Identity;

namespace identity_signup.Areas.Admin.Services
{
    public class RoleService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;

        public RoleService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> CanModifyUserRole(string currentUserId, string targetUserId, string roleId)
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            var role = await _roleManager.FindByIdAsync(roleId);

            if (currentUser == null || targetUser == null || role == null)
                return false;

            // Root Admin kontrolü
            if (await _userManager.IsInRoleAsync(targetUser, "Root Admin"))
                return false;

            // Mevcut kullanıcının en yüksek yetki seviyesini al
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);
            var currentUserHighestPermission = 0;
            foreach (var roleName in currentUserRoles)
            {
                var userRole = await _roleManager.FindByNameAsync(roleName);
                if (userRole != null && userRole.PermissionLevel > currentUserHighestPermission)
                {
                    currentUserHighestPermission = userRole.PermissionLevel;
                }
            }

            // Hedef kullanıcının en yüksek yetki seviyesini al
            var targetUserRoles = await _userManager.GetRolesAsync(targetUser);
            var targetUserHighestPermission = 0;
            foreach (var roleName in targetUserRoles)
            {
                var userRole = await _roleManager.FindByNameAsync(roleName);
                if (userRole != null && userRole.PermissionLevel > targetUserHighestPermission)
                {
                    targetUserHighestPermission = userRole.PermissionLevel;
                }
            }

            // Kullanıcı sadece kendi yetki seviyesinden düşük rolleri değiştirebilir
            return currentUserHighestPermission > role.PermissionLevel && 
                   currentUserHighestPermission > targetUserHighestPermission;
        }
    }
} 