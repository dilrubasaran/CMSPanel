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

        //Kullanýcýnýn sahip olduðu claims bilgilerine göre eriþebileceði menüleri getirir
        public async Task<List<MenuItem>> GetMenuItemsByClaims(IEnumerable<Claim> claims)
        {
            var allMenuItems = await _menuRepository.GetAllMenusAsync();
            return allMenuItems.Where(menu => claims.Any(claim => claim.Type == "MenuAccess" && claim.Value == menu.Id.ToString())).ToList();
        }

        // Kullanýcýnýn rolüne göre menüleri getirir
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