using Microsoft.AspNetCore.Identity;
using TS.Result;
using identity_singup.ViewModels;
using Microsoft.EntityFrameworkCore;
using identity_singup.Models;

namespace identity_signup.Services
{
    public class UserServices
    {
        private readonly UserManager<AppUser> _userManager;

        public UserServices(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<T>> ValidateUserAsync<T>(SignUpViewModel model, T successData = default)
        {
            var errors = new List<string>();

            
            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
            {
                errors.Add("Bu kullanıcı adı zaten alınmış.");
            }

         
            existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                errors.Add("Bu email adresi zaten kullanılıyor.");
            }

          
            existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Phone);
            if (existingUser != null)
            {
                errors.Add("Bu telefon numarası zaten kayıtlı.");
            }

            if (errors.Any())
            {
                return Result<T>.Failure(errors);
            }

            return Result<T>.Succeed(successData);
        }
    }
}
