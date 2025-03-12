using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using identity_singup.Areas.Admin.Services;
using identity_singup.Areas.Admin.ViewModels;
using identity_singup.Models;
using System.Collections.Generic;
using identity_signup.Areas.Instructor.Services;

namespace identity_singup.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class PermissionController : Controller
    {
        private readonly IPermissionRequestService _permissionService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEducationServices _educationService;

        public PermissionController(
            IPermissionRequestService permissionService,
            UserManager<AppUser> userManager,
            IEducationServices educationService)
        {
            _permissionService = permissionService;
            _userManager = userManager;
            _educationService = educationService;
        }


        //Onay bekleyen izin taleplerini alır ve bunları bir liste halinde (View) gönderir
        public async Task<IActionResult> PendingRequests()
        {
            var requests = await _permissionService.GetPendingRequests();
            var viewModels = new List<PendingRequestViewModel>();

            foreach (var request in requests)
            {
                var instructor = await _userManager.FindByIdAsync(request.RequestedBy);
                var education = await _educationService.GetEducationById(request.EducationId);

                viewModels.Add(new PendingRequestViewModel
                {
                    RequestId = request.Id,
                    InstructorName = instructor?.UserName ?? "Bilinmiyor",
                    EduName = education?.EduName ?? "Silinmiş Eğitim",
                    RequestDate = request.RequestDate,
                    Reason = request.Reason
                });
            }

            return View(viewModels);
        }

        // Belirtilen izin talebini onaylar
        [HttpPost]
        public async Task<IActionResult> ApproveRequest(int requestId)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = await _permissionService.ApproveRequest(requestId, user.Id);
            
            if (result)
            {
                TempData["SuccessMessage"] = "Yetki talebi onaylandı.";
            }
            else
            {
                TempData["ErrorMessage"] = "Yetki talebi onaylanırken bir hata oluştu.";
            }
            
            return RedirectToAction(nameof(PendingRequests));
        }
    }
} 