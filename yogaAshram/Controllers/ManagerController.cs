using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;
using yogaAshram.Services;

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
        
        [Authorize]
        // GET
        public IActionResult Index(string employeeId)
        {
            if (employeeId == null)
                employeeId = _userManager.GetUserId(User);
            
            Employee employee = _db.Users.FirstOrDefault(u => u.Id == employeeId);
            return View(employee);
        }
        
        [HttpGet]
        [Authorize]
        public IActionResult CreateEmployee()
        {
            return View();
        }
        
        [HttpPost]
        [Authorize]
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
                    EmailService emailService = new EmailService();
                    await emailService.SendMessageAsync(employee.Email,
                        "Уведомление от центра Yoga Ashram",
                        $"<b>Ваш emal : </b>{employee.Email} \n <b>" +
                                 $"<b>Ваш пароль : </b>{employee.PasswordHash}<b>");
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
        
        [Authorize]
        public IActionResult EditManager(string id = null)
        {
            Employee manager = null;
            manager = id == null ? _userManager.GetUserAsync(User).Result : _userManager.FindByIdAsync(id).Result;
            EditModelView model = new EditModelView()
            {
                Email = manager.Email,
                UserName = manager.UserName,
                NameSurname = manager.NameSurname,
                Id = manager.Id,
            };
            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditManager(EditModelView model)
        {
            if (ModelState.IsValid)
            {
                Employee manager = await _userManager.FindByIdAsync(model.Id);
                if (manager != null)
                {
                    manager.NameSurname = model.NameSurname;
                    manager.UserName = model.UserName;
                    manager.Email = model.Email;

                    var result = await _userManager.UpdateAsync(manager);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    foreach (var error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
    }
}