using identity_signup.Areas.Instructor.Models;
using identity_singup.Models;
using Microsoft.EntityFrameworkCore;
namespace identity_singup.Areas.Admin.Services
{
    public class PermissionRequestService : IPermissionRequestService
    {
        private readonly AppDbContext _context;

        public PermissionRequestService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> HasValidPermission(int educationId, string userId)
        {
            var request = await _context.PermissionRequests
                .Where(p => p.EducationId == educationId && 
                            p.RequestedBy == userId && 
                            p.IsApproved)
                .OrderByDescending(p => p.ApprovedDate)
                .FirstOrDefaultAsync();

            if (request == null || !request.ApprovedDate.HasValue) 
                return false;
              

            // Onay verildikten sonra 7 gün süre tanı
            var daysSinceApproval = (DateTime.Now - request.ApprovedDate.Value).Days;
            return daysSinceApproval <= 7; // 7 gün
        }

        public async Task<List<PermissionRequest>> GetPendingRequests()
        {
            return await _context.PermissionRequests
                .Where(p => !p.IsApproved)
                .ToListAsync();
        }

        public async Task<bool> ApproveRequest(int requestId, string approverId)
        {
            var request = await _context.PermissionRequests.FindAsync(requestId);
            if (request == null) return false;

            request.IsApproved = true;
            request.ApprovedBy = approverId;
            request.ApprovedDate = DateTime.Now;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateRequest(PermissionRequest request)
        {
            await _context.PermissionRequests.AddAsync(request);
            return await _context.SaveChangesAsync() > 0;
        }

        public Task<PermissionRequest> GetLatestApprovedPermission(int educationId, string userId)
        {
            throw new NotImplementedException();
        }
    }
} 