using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using yogaAshram.Models;
using yogaAshram.Services;

namespace yogaAshram.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly IHostEnvironment _environment;
        private YogaAshramContext _db;

        public EmployeesController(UserManager<Employee> userManager, 
            SignInManager<Employee> signInManager,
            IHostEnvironment environment, 
            YogaAshramContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
            _db = db;
        }
        
        // GET
        public IActionResult Index(long? employeeId)
        {
            if (employeeId == null)
                employeeId = GetUserId.GetCurrentUserId(this.HttpContext);
            
            Employee employee = _db.Users.FirstOrDefault(u => u.Id == employeeId);
            return View(employee);
        }
    }
}