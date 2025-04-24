using Microsoft.AspNetCore.Identity;
using TS.Result;
using identity_singup.ViewModels;
using Microsoft.EntityFrameworkCore;
using identity_singup.Models;
using identity_signup.ViewModels;

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

            // Kullanıcı adı kontrolü
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                errors.Add("Bu kullanıcı adı zaten alınmış.");
            }

            // Email kontrolü
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                errors.Add("Bu email adresi zaten kullanılıyor.");
            }

            // Telefon kontrolü
            if (await _userManager.Users.AnyAsync(u => u.PhoneNumber == model.Phone))
            {
                errors.Add("Bu telefon numarası zaten kayıtlı.");
            }

            if (errors.Any())
            {
                return Result<T>.Failure(errors);
            }

            return Result<T>.Succeed(successData);
        }

        public async Task<Result<bool>> CheckUserProfileUpdateAsync(UserEditViewModel request, AppUser currentUser)
        {
            var failures = new List<string>();

            // 1. Değişiklik var mı kontrolü
            bool isChanged =
                currentUser.UserName != request.UserName ||
                currentUser.Email != request.Email ||
                currentUser.PhoneNumber != request.Phone ||
                currentUser.BirthDate != request.BirthDate ||
                currentUser.City != request.City ||
                currentUser.Gender != request.Gender;

            if (!isChanged)
            {
                return Result<bool>.Succeed(false); // Değişiklik yok
            }

            // 2. Uniqueness kontrolleri

            if (currentUser.UserName != request.UserName)
            {
                var usernameExists = await _userManager.FindByNameAsync(request.UserName);
                if (usernameExists != null && usernameExists.Id !=currentUser.Id)
                {
                    failures.Add("Bu kullanıcı adı başka bir kullanıcı tarafından kullanılıyor.");
                }
            }

            if (currentUser.Email != request.Email)
            {
                var emailOwner = await _userManager.FindByEmailAsync(request.Email);
                if (emailOwner != null && emailOwner.Id != currentUser.Id)
                    failures.Add("Bu e-posta adresi kullanımda.");
            }

            if (currentUser.PhoneNumber != request.Phone)
            {
                var phoneExists = await _userManager.Users.AnyAsync(u => u.PhoneNumber == request.Phone && u.Id != currentUser.Id);
                if (phoneExists)
                    failures.Add("Bu telefon numarası kullanımda.");
            }

            return failures.Count == 0 ? Result<bool>.Succeed(true) : Result<bool>.Failure(failures);
        }
    }
}
