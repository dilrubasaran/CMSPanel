using identity_singup.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace identity_singup.Areas.Admin.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly AppDbContext _context;

        public MenuRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<MenuItem>> GetAllMenusAsync()
        {
            // Önce tüm menü öğelerini getir
            var allMenuItems = await _context.MenuItems
                .OrderBy(m => m.SortNumber)
                .ToListAsync();

            // Menü hiyerarşisini oluştur
            var rootMenuItems = allMenuItems
                .Where(m => m.ParentId == null)
                .ToList();

            // Her bir kök menü için alt menüleri ekle
            foreach (var menuItem in rootMenuItems)
            {
                BuildMenuHierarchy(menuItem, allMenuItems);
            }

            return rootMenuItems;
        }

        public async Task<List<MenuItem>> GetMenusByRoleAsync(string role)
        {
            // Belirli bir role ait menü öğelerini getir
            var menuItems = await _context.MenuItems
                .Where(m => m.Role == role || m.Role == "all")
                .OrderBy(m => m.SortNumber)
                .ToListAsync();

            // Menü hiyerarşisini oluştur
            var rootMenuItems = menuItems
                .Where(m => m.ParentId == null)
                .ToList();

            // Her bir kök menü için alt menüleri ekle
            foreach (var menuItem in rootMenuItems)
            {
                BuildMenuHierarchy(menuItem, menuItems);
            }

            return rootMenuItems;
        }


        // Menü hiyerarşisini oluşturan yardımcı metot
        private void BuildMenuHierarchy(MenuItem parent, List<MenuItem> allMenuItems)
        {
            parent.SubMenuItems = allMenuItems
                .Where(m => m.ParentId == parent.Id)
                .OrderBy(m => m.SortNumber)
                .ToList();

            foreach (var child in parent.SubMenuItems)
            {
                BuildMenuHierarchy(child, allMenuItems);
            }
        }
    }
} 