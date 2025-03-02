using System.Collections.Generic;
using System.Threading.Tasks;
using identity_signup.Areas.Instructor.Models;

namespace identity_singup.Areas.Admin.Services
{
    public interface IPermissionRequestService
    {
        Task<bool> HasValidPermission(int educationId, string userId);
        Task<List<PermissionRequest>> GetPendingRequests();
        Task<bool> ApproveRequest(int requestId, string approverId);
        Task<bool> CreateRequest(PermissionRequest request);

    }
} 