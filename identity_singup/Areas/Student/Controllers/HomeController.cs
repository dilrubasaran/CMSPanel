using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace identity_singup.Areas.Student.Controllers
{
    [Authorize(Roles = "student")]
    [Area("Student")]
    public class HomeController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
    }
} 