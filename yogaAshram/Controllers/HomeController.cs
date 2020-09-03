using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;

namespace yogaAshram.Controllers
{
    public class HomeController : Controller
    {
        // GET
        [DefaultBreadcrumb("Главная")]
        public IActionResult Index()
        {
            return View();
        }
    }
}