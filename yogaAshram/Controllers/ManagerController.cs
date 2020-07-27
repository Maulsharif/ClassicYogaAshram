using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;

namespace yogaAshram.Controllers
{
    [Authorize(Roles = "manager")]
    public class ManagerController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private YogaAshramContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManagerController(
            UserManager<Employee> userManager,
            SignInManager<Employee> signInManager,
            YogaAshramContext db, 
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _roleManager = roleManager;
        }
        
        // GET
        public IActionResult Index(string? employeeId)
        {
            if (employeeId == null)
                employeeId = _userManager.GetUserId(User);
            
            Employee employee = _db.Users.FirstOrDefault(u => u.Id == employeeId);
            return View(employee);
        }
        
        [HttpGet]
        public IActionResult CreateEmployee()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateEmployee(AccountCreateModelView model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = new Employee
                {
                    Email = model.Email,
                    UserName = model.UserName,
                    NameSurname = model.NameSurname
                };
                var result = await _userManager.CreateAsync(employee, model.Password);
                if (result.Succeeded)
                {
                    IdentityRole role = await _roleManager.FindByNameAsync(model.Role);
                    await _userManager.AddToRoleAsync(employee, role.Name);
                    await _signInManager.SignInAsync(employee, false);
                    return RedirectToAction("Index", "Employees");
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}