using identity_singup.Models;
using System.Security.Claims;

public interface IMenuService
{
    Task<List<MenuItem>> GetMenuItemsByClaims(IEnumerable<Claim> claims);
    Task<List<MenuItem>> GetMenuItemsByRole(string role);
    string GetMenuUrl(MenuItem menuItem);
}