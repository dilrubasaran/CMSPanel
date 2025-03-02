using identity_signup.Areas.Instructor.Models;
using identity_signup.Areas.Instructor.ViewModels;
using identity_singup.Areas.Admin.Services;
using identity_singup.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using identity_signup.Areas.Instructor.Services;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace identity_signup.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = "instructor,admin")]
    public class EduController : Controller
    {
        private readonly IEducationServices _educationService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IPermissionRequestService _permissionService;

        public EduController(
            IEducationServices educationService, 
            UserManager<AppUser> userManager,
            IAuthorizationService authorizationService,
            IPermissionRequestService permissionService)
        {
            _educationService = educationService;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _permissionService = permissionService;
        }

        private void LoadEduTypes()
        {
            var eduTypes = Enum.GetValues(typeof(EduType))
                .Cast<EduType>()
                .Select(e => new SelectListItem
                {
                    Value = ((int)e).ToString(),
                    Text = e.ToString()
                })
                .ToList();

            ViewBag.EduTypes = eduTypes;
        }

        public async Task<IActionResult> EduList()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var educations = User.IsInRole("admin") 
                ? await _educationService.GetAllEducations()
                : await _educationService.GetInstructorEducations(user.Id);

            return View(educations);
        }

        public IActionResult EduCreate()
        {
            var user = _userManager.GetUserAsync(User).Result;
            var model = new EduCreateViewModel
            {
                CreatedBy = user.UserName // Kullanıcı adını otomatik set et
            };
            //alternatif ne kullanılabiir
            LoadEduTypes();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EduCreate(EduCreateViewModel model)
        {
            try 
            {
                if (!ModelState.IsValid)
                {
                    LoadEduTypes(); // Form geçersizse dropdown'ı tekrar yükle
                    return View(model);
                }

                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                var result = await _educationService.AddEducation(model, user.Id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Eğitim başarıyla oluşturuldu.";
                    return RedirectToAction(nameof(EduList));
                }

                LoadEduTypes();
                ModelState.AddModelError("", "Eğitim oluşturulurken bir hata oluştu.");
                return View(model);
            }
            catch (Exception ex)
            {
                LoadEduTypes();
                ModelState.AddModelError("", $"Beklenmeyen bir hata oluştu: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EduUpdate(int id)
        {
            var education = await _educationService.GetEducationById(id);
            if (education == null) return NotFound();

            var authResult = await _authorizationService.AuthorizeAsync(
                User, education, "CanEditEducation");

            if (!authResult.Succeeded)
            {
                if (User.IsInRole("instructor"))
                {
                    return RedirectToAction(nameof(RequestPermission), new { id = education.Id });
                }
                return Forbid();
            }

            var viewModel = new EduUpdateViewModel
            {
                Id = education.Id,
                EduName = education.EduName,
                Description = education.Description,
                EduType = education.EduType,
                EduDuration = education.EduDuration,
                EduPrice = education.EduPrice
            };

            LoadEduTypes();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EduUpdate(EduUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                LoadEduTypes();
                return View(model);
            }

            var result = await _educationService.UpdateEducation(model);
            if (result)
            {
                TempData["SuccessMessage"] = "Eğitim başarıyla güncellendi.";
                return RedirectToAction(nameof(EduList));
            }

            LoadEduTypes();
            ModelState.AddModelError("", "Eğitim güncellenirken bir hata oluştu.");
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EduDelete(int id)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return NotFound();

                var education = await _educationService.GetEducationById(id);
                if (education == null) return NotFound();

                // Admin değilse ve eğitim kendisine ait değilse erişimi engelle
                if (!User.IsInRole("admin") && education.CreatedBy != user.Id)
                    return Forbid();

                var result = await _educationService.DeleteEducation(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Eğitim başarıyla silindi.";
                    return RedirectToAction(nameof(EduList));
                }

                TempData["ErrorMessage"] = "Eğitim silinirken bir hata oluştu.";
                return RedirectToAction(nameof(EduList));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Beklenmeyen bir hata oluştu: {ex.Message}";
                return RedirectToAction(nameof(EduList));
            }
        }

        [HttpGet]
        public async Task<IActionResult> RequestPermission(int id)
        {
            var education = await _educationService.GetEducationById(id);
            if (education == null) return NotFound();

            var viewModel = new PermissionRequestViewModel
            {
                EducationId = education.Id,
                EduName = education.EduName
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RequestPermission(PermissionRequestViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var request = new PermissionRequest
            {
                EducationId = model.EducationId,
                RequestedBy = user.Id,
                Reason = model.Reason,
                RequestDate = DateTime.Now
            };

            var result = await _permissionService.CreateRequest(request);
            if (result)
            {
                TempData["SuccessMessage"] = "Güncelleme talebiniz admin'e iletildi.";
                return RedirectToAction(nameof(EduList));
            }

            ModelState.AddModelError("", "Talep gönderilirken bir hata oluştu.");
            return View(model);
        }
    }
}
