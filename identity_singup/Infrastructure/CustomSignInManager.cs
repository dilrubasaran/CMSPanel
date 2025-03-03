using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using identity_singup.Models;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;

namespace identity_singup.Infrastructure
{
    public class CustomSignInManager : SignInManager<AppUser>
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CustomSignInManager(
            UserManager<AppUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<AppUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<AppUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<AppUser> confirmation,
            AppDbContext context)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
            _context = context;
            _httpContextAccessor = contextAccessor;
        }

        private string GetUserIpAddress()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null) return "Unknown";

            // X-Forwarded-For header'ını kontrol et (proxy veya load balancer durumu için)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                // İlk IP adresini al (birden fazla proxy olabilir)
                return forwardedFor.Split(',')[0].Trim();
            }

            // Remote IP adresini al
            var remoteIp = httpContext.Connection.RemoteIpAddress;
            if (remoteIp != null)
            {
                // IPv6 ise IPv4'e dönüştürmeyi dene
                if (remoteIp.IsIPv4MappedToIPv6)
                {
                    remoteIp = remoteIp.MapToIPv4();
                }
                return remoteIp.ToString();
            }

            return "Unknown";
        }

        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);
            
            // Aktif/Pasif kontrolü
            if (user != null && !user.IsActive)
            {
                await LogLoginAttempt(userName, false, "Hesap pasif durumda");
                return SignInResult.NotAllowed;
            }

            // Kullanıcı adı ve şifre kontrolü
            var result = await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
            
            await LogLoginAttempt(userName, result.Succeeded, !result.Succeeded ? GetFailureMessage(result) : "");
            
            return result;
        }

        //Giriş denemelerini kaydeder
        private async Task LogLoginAttempt(string userName, bool isSuccess, string failureMessage)
        {
            var user = await UserManager.FindByNameAsync(userName);
            var roles = user != null ? await UserManager.GetRolesAsync(user) : new List<string>();
            
            var audit = new LoginAudit
            {
                UserId = user?.Id ?? "Anonymous",
                UserName = userName,
                LoginTime = DateTime.Now,
                IpAddress = GetUserIpAddress(),
                UserAgent = _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown",
                IsSuccess = isSuccess,
                FailureMessage = failureMessage,
                UserRole = roles.FirstOrDefault() ?? ""
            };
            //giriş denemelerini db ye kaydeder 
            await _context.LoginAudits.AddAsync(audit);
            await _context.SaveChangesAsync();
        }

        //Başarısız giriş denemesi sonucuna göre hata mesajını döndürür
        private string GetFailureMessage(SignInResult result)
        {
            if (result.IsLockedOut) return "Hesap kilitli";
            if (result.IsNotAllowed) return "Giriş izni yok";
            if (result.RequiresTwoFactor) return "İki faktörlü doğrulama gerekli";
            return "Geçersiz kullanıcı adı veya şifre";
        }
    }
} 