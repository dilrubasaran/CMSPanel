using Microsoft.AspNetCore.Authorization;

namespace identity_singup.Infrastructure.Authorization
{
    public class CanEditEducationPolicy : IAuthorizationRequirement
    {
        public int AllowedDaysForEdit { get; }

    
        public CanEditEducationPolicy(int allowedDaysForEdit = 7) // 7 gün
        {
            AllowedDaysForEdit = allowedDaysForEdit;
        }
    }
} 