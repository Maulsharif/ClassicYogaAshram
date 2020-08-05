using Microsoft.AspNetCore.Mvc;

namespace yogaAshram.Controllers
{
    public class ClientsController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}