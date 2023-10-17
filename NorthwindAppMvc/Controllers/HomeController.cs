using Microsoft.AspNetCore.Mvc;

namespace NorthwindAppMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Chat()
        {
            return View(nameof(Chat));
        }
    }
}
