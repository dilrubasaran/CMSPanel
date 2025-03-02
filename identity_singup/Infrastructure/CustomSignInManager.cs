using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using identity_singup.Models;
using Microsoft.AspNetCore.Authentication;

namespace identity_signup.Infrastructure
{
    public class CustomSignInManager : SignInManager<AppUser>
    {
        public CustomSignInManager(
            UserManager<AppUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<AppUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<AppUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<AppUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        //Kulanýcý giriþ yaparken IsActive özelliði false olan kullanýcýlarýn giriþ yapmasýný engeller
        public override async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            var user = await UserManager.FindByNameAsync(userName);
            if (user != null && !user.IsActive)
            {
                return SignInResult.NotAllowed;
            }

            return await base.PasswordSignInAsync(userName, password, isPersistent, lockoutOnFailure);
        }
    }
}