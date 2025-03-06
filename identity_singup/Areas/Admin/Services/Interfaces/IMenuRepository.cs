using System.Collections.Generic;
using System.Threading.Tasks;
using identity_singup.Models;


namespace identity_singup.Areas.Admin.Repositories
{
    public interface IMenuRepository
    {
        // Tüm menü öğelerini getir
        Task<List<MenuItem>> GetAllMenusAsync();
        
        // Belirli bir role ait menü öğelerini getir
        Task<List<MenuItem>> GetMenusByRoleAsync(string role);
        
       
    }
}