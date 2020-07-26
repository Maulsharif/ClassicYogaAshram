using Microsoft.AspNetCore.Mvc;

namespace yogaAshram.Controllers
{
    public class ManagerController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}