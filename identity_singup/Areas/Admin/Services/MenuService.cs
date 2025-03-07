using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using identity_singup.Areas.Admin.Repositories;
using identity_singup.Models;

namespace identity_singup.Areas.Admin.Services
{
 

    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        //Kullan�c�n�n sahip oldu�u claims bilgilerine g�re eri�ebilece�i men�leri getirir
        public async Task<List<MenuItem>> GetMenuItemsByClaims(IEnumerable<Claim> claims)
        {
            var allMenuItems = await _menuRepository.GetAllMenusAsync();
            return allMenuItems.Where(menu => claims.Any(claim => claim.Type == "MenuAccess" && claim.Value == menu.Id.ToString())).ToList();
        }

        // Kullan�c�n�n rol�ne g�re men�leri getirir
        public async Task<List<MenuItem>> GetMenuItemsByRole(string role)
        {
            return await _menuRepository.GetMenusByRoleAsync(role);
        }

        public string GetMenuUrl(MenuItem menuItem)
        {
            return string.IsNullOrEmpty(menuItem.AreaName) 
                ? $"/{menuItem.ControllerName}/{menuItem.ActionName}" 
                : $"/{menuItem.AreaName}/{menuItem.ControllerName}/{menuItem.ActionName}";
        }
    }
}