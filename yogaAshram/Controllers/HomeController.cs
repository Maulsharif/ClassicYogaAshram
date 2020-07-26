using Microsoft.AspNetCore.Mvc;

namespace yogaAshram.Controllers
{
    public class HomeController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}