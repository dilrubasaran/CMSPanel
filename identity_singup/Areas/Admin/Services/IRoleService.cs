using identity_singup.Models;

namespace identity_signup.Areas.Admin.Services
{
    public interface IRoleService
    {
        Task<bool> CanModifyUserRole(string currentUserId, string targetUserId, string roleId);
        Task<bool> IsRootAdmin(AppUser user);
        Task<int> GetUserHighestPermissionLevel(string userId);
        Task<bool> HasPermissionToModifyRole(string currentUserId, string roleId);
        Task<List<AppRole>> GetAllRoles();
        Task<bool> UpdateRolePermissionLevel(string roleId, int permissionLevel, string currentUserId);
    }
} 