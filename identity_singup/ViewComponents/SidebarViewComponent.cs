using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using identity_singup.Models;
using identity_singup.Areas.Admin.Services;
using identity_singup.Areas.Admin.Repositories;
using Microsoft.AspNetCore.Identity;

namespace identity_singup.ViewComponents
{
    public class SidebarViewComponent : ViewComponent
    {
        private readonly IMenuService _menuService;
        private readonly IClaimsService _claimsService;

        public SidebarViewComponent(IMenuService menuService, IClaimsService claimsService)
        {
            _menuService = menuService;
            _claimsService = claimsService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Kullanıcının rollerini al
            var userRoles = _claimsService.GetUserRoles();
            
            // Kullanıcı rollerine göre menü öğelerini getir
            var menuItems = new List<MenuItem>();
            
            // Kullanıcı giriş yapmışsa
            if (User.Identity.IsAuthenticated)
            {
                // Kullanıcının claim'lerini al
                var userClaims = User is ClaimsPrincipal claimsPrincipal 
                    ? claimsPrincipal.Claims 
                    : Enumerable.Empty<Claim>();
                
                // Kullanıcının erişebileceği menü öğelerini getir
                menuItems = await _menuService.GetMenuItemsByClaims(userClaims);
                
                // Eğer menü öğeleri boşsa, rol bazlı menüleri getir
                if (!menuItems.Any() && userRoles.Any())
                {
                    // Her bir rol için menü öğelerini getir
                    foreach (var role in userRoles)
                    {
                        var roleMenuItems = await _menuService.GetMenuItemsByRole(role);
                        menuItems.AddRange(roleMenuItems);
                    }
                }
                
                // Aktif menü öğesini belirle
                var currentUrl = HttpContext.Request.Path;
                SetActiveMenuItem(menuItems, currentUrl);
            }

            return View(menuItems);
        }

        private void SetActiveMenuItem(List<MenuItem> menuItems, string currentUrl)
        {
            foreach (var item in menuItems)
            {
                // Menü URL'sini oluştur
                var menuUrl = _menuService.GetMenuUrl(item);
                
                // Aktif menü kontrolü
                item.IsActive = menuUrl.Equals(currentUrl.ToString(), StringComparison.OrdinalIgnoreCase);
                
                // Alt menüler için de kontrol et
                if (item.SubMenuItems?.Any() == true)
                {
                    SetActiveMenuItem(item.SubMenuItems, currentUrl);
                    
                    // Eğer alt menülerden biri aktifse, üst menüyü de aktif yap
                    if (item.SubMenuItems.Any(sm => sm.IsActive))
                    {
                        item.IsActive = true;
                    }
                }
            }
        }
    }
} 