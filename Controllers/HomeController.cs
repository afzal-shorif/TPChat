using Microsoft.AspNetCore.Mvc;

namespace TPChat.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
