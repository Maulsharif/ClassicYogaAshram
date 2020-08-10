using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using yogaAshram.Models;

namespace yogaAshram.Controllers
{
    [Authorize(Roles = "admin,chief")]
    public class AdminController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly YogaAshramContext _db;

        public AdminController(SignInManager<Employee> signInManager, YogaAshramContext db, UserManager<Employee> userManager)
        {
            _signInManager = signInManager;
            _db = db;
            _userManager = userManager;
        }

        // GET
        public async Task<IActionResult> Index()
        { Employee admin = await _userManager.GetUserAsync(User);
            return View(admin);
        }
        
    }
}