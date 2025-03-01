using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace identity_singup.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = "instructor")] 
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

      
    }
} 