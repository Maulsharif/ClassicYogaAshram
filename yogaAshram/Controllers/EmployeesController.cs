using Microsoft.AspNetCore.Mvc;

namespace yogaAshram.Controllers
{
    public class EmployeesController : Controller
    {
        // GET
        public IActionResult Index()
        {
            return View();
        }
    }
}