using Microsoft.AspNetCore.Authorization;

namespace identity_singup.Infrastructure.Authorization
{
    public class CanEditEducationPolicy : IAuthorizationRequirement
    {
        public int AllowedMinutesForEdit { get; }
// public CanEditEducationPolicy(int allowedDaysForEdit = 7)
//     {
        // AllowedDaysForEdit = allowedDaysForEdit;
        public CanEditEducationPolicy(int allowedMinutesForEdit = 1)
        {
            AllowedMinutesForEdit = allowedMinutesForEdit;
        }
    }
} 