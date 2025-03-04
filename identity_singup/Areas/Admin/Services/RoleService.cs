using identity_singup.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace identity_signup.Areas.Admin.Services
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private const int ROOT_ADMIN_PERMISSION = 100;
        private const int ADMIN_PERMISSION = 50;
        private const int INSTRUCTOR_PERMISSION = 30;
        private const int STUDENT_PERMISSION = 10;

        public RoleService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<bool> IsRootAdmin(AppUser user)
        {
            return await _userManager.IsInRoleAsync(user, "Root Admin");
        }

        public async Task<bool> IsAdmin(AppUser user)
        {
            return await _userManager.IsInRoleAsync(user, "admin");
        }

        public async Task<int> GetUserHighestPermissionLevel(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return 0;

            var userRoles = await _userManager.GetRolesAsync(user);
            var highestPermission = 0;

            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null && role.PermissionLevel > highestPermission)
                {
                    highestPermission = role.PermissionLevel;
                }
            }

            return highestPermission;
        }

        public async Task<bool> HasPermissionToModifyRole(string currentUserId, string roleId)
        {
            var currentUserPermissionLevel = await GetUserHighestPermissionLevel(currentUserId);
            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null) return false;

            return currentUserPermissionLevel > role.PermissionLevel;
        }

        public async Task<bool> CanModifyUserRole(string currentUserId, string targetUserId, string roleId)
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            var targetUser = await _userManager.FindByIdAsync(targetUserId);
            var role = await _roleManager.FindByIdAsync(roleId);

            if (currentUser == null || targetUser == null || role == null)
                return false;

            // Kullanıcı kendi rolünü değiştiremez
            if (currentUserId == targetUserId)
                return false;

            // Root Admin kontrolü
            bool isCurrentUserRootAdmin = await IsRootAdmin(currentUser);
            bool isTargetUserRootAdmin = await IsRootAdmin(targetUser);
            bool isTargetUserAdmin = await IsAdmin(targetUser);

            // Root Admin, herhangi bir kullanıcıya herhangi bir rol atayabilir (kendi rolü ve Root Admin rolü hariç)
            if (isCurrentUserRootAdmin)
            {
                // Root Admin rolünü kimseye atayamaz
                if (role.Name == "Root Admin")
                    return false;
                
                // Admin ve Root Admin'e rol atayabilir
                return true;
            }

            // Admin, Admin ve Root Admin'e rol atayamaz
            bool isCurrentUserAdmin = await IsAdmin(currentUser);
            if (isCurrentUserAdmin)
            {
                if (isTargetUserAdmin || isTargetUserRootAdmin)
                    return false;

                // Admin, Root Admin rolünü atayamaz
                if (role.Name == "Root Admin")
                    return false;

                // Admin, admin rolünü atayamaz
                if (role.Name == "admin")
                    return false;

                return true;
            }

            return false;
        }

        public async Task<List<AppRole>> GetAllRoles()
        {
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<bool> UpdateRolePermissionLevel(string roleId, int permissionLevel, string currentUserId)
        {
            var currentUser = await _userManager.FindByIdAsync(currentUserId);
            if (currentUser == null) return false;
            
            // Sadece root admin rol yetkilerini değiştirebilir
            if (!await IsRootAdmin(currentUser)) return false;
            
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return false;
            
            // Root admin rolünün yetkisi değiştirilemez
            if (role.Name.ToLower() == "root admin") return false;
            
            role.PermissionLevel = permissionLevel;
            var result = await _roleManager.UpdateAsync(role);
            
            return result.Succeeded;
        }
    }
} 