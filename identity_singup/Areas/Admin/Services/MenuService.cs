using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using identity_singup.Areas.Admin.Repositories;
using identity_singup.Models;

namespace identity_singup.Areas.Admin.Services
{
    public interface IMenuService
    {
        Task<List<MenuItem>> GetMenuItemsByClaims(IEnumerable<Claim> claims);
        Task<List<MenuItem>> GetMenuItemsByRole(string role);
        string GetMenuUrl(MenuItem menuItem);
    }

    public class MenuService : IMenuService
    {
        private readonly IMenuRepository _menuRepository;

        public MenuService(IMenuRepository menuRepository)
        {
            _menuRepository = menuRepository;
        }

        public async Task<List<MenuItem>> GetMenuItemsByClaims(IEnumerable<Claim> claims)
        {
            var allMenuItems = await _menuRepository.GetAllMenusAsync();
            return allMenuItems.Where(menu => claims.Any(claim => claim.Type == "MenuAccess" && claim.Value == menu.Id.ToString())).ToList();
        }

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