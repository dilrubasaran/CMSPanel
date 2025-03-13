using identity_singup.Areas.Admin.Repositories;
using identity_singup.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace identity_signup.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin,Root Admin")]
    public class MenuController : Controller
    {
        private readonly IMenuRepository _menuRepository;
        private readonly IMenuService _menuService;

        public MenuController(IMenuRepository menuRepository, IMenuService menuService)
        {
            _menuRepository = menuRepository;
            _menuService = menuService;
        }

        public async Task<IActionResult> MenuAdd()
        {
            var model = new MenuAddViewModel
            {
                ParentMenus = await _menuRepository.GetAllMenusAsync(),
                Roles = new List<string> { "Root Admin", "Admin", "Instructor", "Student" }
            };
            return View(model);
        }

        [HttpPost]
      public async Task<IActionResult> MenuAdd(MenuAddViewModel model)
{
    if (!ModelState.IsValid)
    {
        try
        {
            var menuItem = new MenuItem
            {
                Name = model.Name,
                Icon = model.Icon,
                ControllerName = model.ControllerName,
                ActionName = model.ActionName,
                AreaName = model.AreaName,
                ParentId = model.ParentId,
                Role = model.Role,
                SortNumber = model.SortNumber
            };

            Console.WriteLine("MenuAdd çağrılıyor...");

            await _menuRepository.MenuAdd(menuItem);

            Console.WriteLine("MenuAdd tamamlandı!");

            return RedirectToAction("MenuList");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Hata oluştu: " + ex.Message);
            ModelState.AddModelError("", "Bir hata oluştu: " + ex.Message);
        }
    }

    model.ParentMenus = await _menuRepository.GetAllMenusAsync();
    model.Roles = new List<string> { "Root Admin", "Admin", "Instructor", "Student" };
    return View(model);
}


        public IActionResult MenuUpdate()
        {
            return View();
        }

        public IActionResult MenuRemove()
        {
            return View();
        }


        public IActionResult MenuList()
        {

            return View();
        }
    }
}
