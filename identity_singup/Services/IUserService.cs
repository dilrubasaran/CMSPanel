using identity_singup.ViewModels;
using identity_signup.ViewModels;
using identity_singup.Models;
using TS.Result;

namespace identity_signup.Services
{
    public interface IUserService
    {
        Task<Result<T>> ValidateUserAsync<T>(SignUpViewModel model, T successData = default);
        Task<Result<bool>> CheckUserProfileUpdateAsync(UserEditViewModel request, AppUser currentUser);
    }
} 