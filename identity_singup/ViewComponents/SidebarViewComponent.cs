using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using identity_singup.Models;

namespace identity_singup.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly UserManager<AppUser> _userManager;

        public SidebarViewComponent(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(UserClaimsPrincipal);
            if (user == null) return View(new List<MenuItem>());

            var roles = await _userManager.GetRolesAsync(user);
            var menuItems = GetMenuItemsByRole(roles);

            // Aktif menü öğesini belirle
            var currentUrl = HttpContext.Request.Path;
            SetActiveMenuItem(menuItems, currentUrl);

            return View(menuItems);
        }

        private void SetActiveMenuItem(List<MenuItem> menuItems, string currentUrl)
        {
            foreach (var item in menuItems)
            {
                item.IsActive = item.Url?.Equals(currentUrl.ToString(), StringComparison.OrdinalIgnoreCase) ?? false;
                if (item.SubItems?.Any() == true)
                {
                    SetActiveMenuItem(item.SubItems, currentUrl);
                }
            }
        }

        private List<MenuItem> GetMenuItemsByRole(IList<string> roles)
        {
            var menuItems = new List<MenuItem>();

            if (roles.Contains("admin"))
            {
                menuItems.AddRange(new[]
                {
                    new MenuItem { Title = "Dashboard", Icon = "bi bi-speedometer2", Url = "/Admin/Home/Index" },
                    new MenuItem { Title = "Kullanıcı Yönetimi", Icon = "bi bi-people", Url = "/Admin/Home/UserList" },
                    new MenuItem { Title = "Rol Yönetimi", Icon = "bi bi-shield-lock", Url = "/Admin/Role/Index" },
                    new MenuItem 
                    { 
                        Title = "Eğitim Yönetimi", 
                        Icon = "bi bi-mortarboard",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Eğitimler", Icon = "bi bi-collection", Url = "/Admin/Course/Index" },
                            new MenuItem { Title = "Kategoriler", Icon = "bi bi-tags", Url = "/Admin/Category/Index" }
                        }
                    },
                    new MenuItem 
                    { 
                        Title = "Raporlar", 
                        Icon = "bi bi-graph-up",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Satış Raporu", Icon = "bi bi-cash", Url = "/Admin/Report/Sales" },
                            new MenuItem { Title = "Kullanıcı Raporu", Icon = "bi bi-person-lines-fill", Url = "/Admin/Report/Users" }
                        }
                    }
                });
            }

            if (roles.Contains("instructor"))
            {
                menuItems.AddRange(new[]
                {
                    new MenuItem { Title = "Dashboard", Icon = "bi bi-speedometer2", Url = "/Instructor/Home/Index" },
                    new MenuItem { Title = "Eğitimlerim", Icon = "bi bi-collection-play", Url = "/Instructor/Course/MyCourses" },
                    new MenuItem { Title = "Yeni Eğitim", Icon = "bi bi-plus-circle", Url = "/Instructor/Course/Create" },
                    new MenuItem 
                    { 
                        Title = "Öğrenci Yönetimi", 
                        Icon = "bi bi-people",
                        SubItems = new List<MenuItem>
                        {
                            new MenuItem { Title = "Öğrenci Listesi", Icon = "bi bi-person-lines-fill", Url = "/Instructor/Student/List" },
                            new MenuItem { Title = "Başarı Durumu", Icon = "bi bi-graph-up", Url = "/Instructor/Student/Progress" }
                        }
                    }
                });
            }

            if (roles.Contains("student"))
            {
                menuItems.AddRange(new[]
                {
                    new MenuItem { Title = "Dashboard", Icon = "bi bi-speedometer2", Url = "/Student/Home/Index" },
                    new MenuItem { Title = "Kurslarım", Icon = "bi bi-play-circle", Url = "/Student/Course/MyCourses" },
                    new MenuItem { Title = "Sertifikalarım", Icon = "bi bi-award", Url = "/Student/Certificate/Index" },
                    new MenuItem { Title = "Başarı Durumum", Icon = "bi bi-graph-up", Url = "/Student/Progress/Index" }
                });
            }

            return menuItems;
        }
    }
} 