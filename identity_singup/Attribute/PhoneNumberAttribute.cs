using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Extensions;

namespace identity_signup.Attribute
{
    public class PhoneNumberAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var userClaims = filterContext.HttpContext.User;
            if (!userClaims.Identity.IsAuthenticated )
            {
                filterContext.Result = new RedirectToRouteResult(
                    "Login",
                    new RouteValueDictionary(new { controller = "Home", action = "SignIn" })
                );
                return;
            }

            var identity = (ClaimsIdentity)userClaims.Identity;
            var phoneClaim = identity.FindFirst("PhoneNumber");

            if (phoneClaim == null || string.IsNullOrEmpty(phoneClaim.Value))
            {
                var returnUrl = filterContext.HttpContext.Request.Path + filterContext.HttpContext.Request.QueryString;
                if (filterContext.Controller is Controller controller)
                {
                    controller.TempData["ReturnUrl"] = returnUrl;
                }

                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    {"area", ""},
                    { "controller", "Member" },
                    { "action", "UserEdit" }
                });

                return;
            }
           



        }
    }
}
