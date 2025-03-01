using identity_signup.Areas.Instructor.Models;
using identity_signup.Areas.Instructor.ViewModels;
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
    [Authorize(Roles = "instructor")]
    public class EduController : Controller
    {
        private readonly IEducationServices _educationService;
        private readonly UserManager<AppUser> _userManager;

        public EduController(IEducationServices educationService, UserManager<AppUser> userManager)
        {
            _educationService = educationService;
            _userManager = userManager;
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
            var educations = await _educationService.GetAllEducations();
            return View(educations);
        }

        public IActionResult EduCreate()
        {
            //ne dememk bak 
            LoadEduTypes();
            return View();
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

        public async Task<IActionResult> EduUpdate(int id)
        {
            var education = await _educationService.GetEducationById(id);
            if (education == null)
                return NotFound();

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
    }
}
