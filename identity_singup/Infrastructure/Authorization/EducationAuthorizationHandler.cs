using identity_signup.Areas.Instructor.Models;
using identity_singup.Areas.Admin.Services;
using identity_singup.Infrastructure.Authorization;
using identity_singup.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Authorization
{
    public class EducationAuthorizationHandler : AuthorizationHandler<CanEditEducationPolicy, Education>
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IPermissionRequestService _permissionService;

        public EducationAuthorizationHandler(
            UserManager<AppUser> userManager,
            IPermissionRequestService permissionService)
        {
            _userManager = userManager;
            _permissionService = permissionService;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            CanEditEducationPolicy requirement,
            Education resource)
        {
            var user = await _userManager.GetUserAsync((ClaimsPrincipal)context.User);
            if (user == null) return;

            if (context.User.IsInRole("admin"))
            {
                context.Succeed(requirement);
                return;
            }

            if (resource.CreatedBy != user.Id) return;

            var minutesSinceCreation = (DateTime.Now - resource.CreatedAt).TotalMinutes;
            if (minutesSinceCreation <= requirement.AllowedMinutesForEdit)
            {
                context.Succeed(requirement);
            }

            var hasExtendedPermission = await _permissionService
                .HasValidPermission(resource.Id, user.Id);
            
            if (hasExtendedPermission)
            {
                context.Succeed(requirement);
            }
        }
    }
} 