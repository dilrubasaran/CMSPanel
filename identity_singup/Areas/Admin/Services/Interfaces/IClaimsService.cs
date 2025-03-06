using System.Collections.Generic;
using System.Security.Claims;

namespace identity_singup.Areas.Admin.Services
{
    public interface IClaimsService
    {
        // Kullanıcı ID'sini al
        string GetUserId();
        
        // Kullanıcı adını al
        string GetUserName();
        
        // Kullanıcı rollerini al
        List<string> GetUserRoles();
        
        // Kullanıcının belirli bir claim'e sahip olup olmadığını kontrol et
        bool HasClaim(string claimType, string claimValue);
    }
}
