using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        private readonly RoleManager<Role> _roleManager;

        public ManagerController(
            UserManager<Employee> userManager,
            SignInManager<Employee> signInManager,
            YogaAshramContext db, 
            RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _roleManager = roleManager;
        }
        
        [Authorize]
        // GET
        public async Task<IActionResult> Index()
        {
            Employee empl = await _userManager.GetUserAsync(User);
            await SetViewBagRoles();
            return View(new ManagerIndexModelView() { Employee = empl });
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
                string newPsw = PasswordGenerator.Generate();
                var result = await _userManager.CreateAsync(employee, newPsw);
                if (result.Succeeded)
                {
                    Role role = await _roleManager.FindByNameAsync(model.Role);
                    await _userManager.AddToRoleAsync(employee, role.Name);
                    ManagerEmployee managerEmployee = new ManagerEmployee()
                    {
                        Manager = _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext)),
                        Employee = employee
                    };
                    _db.Entry(managerEmployee).State = EntityState.Added;
                    await _db.SaveChangesAsync();
                    await EmailService.SendMessageAsync(employee.Email,
                            "Уведомление от центра Yoga Ashram",
                            $"<b>Ваш emal : </b>{employee.Email} \n <b>" + 
                            $"<b>Ваш пароль : </b> {newPsw} <b>");
                    await _signInManager.SignInAsync(employee, false);
                    return Json("true");
                }
                
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
        
        public async Task<IActionResult> GetEditModalAjax()
        {
            Employee employee = await _userManager.GetUserAsync(User);
            return PartialView("PartialViews/ManagerAccountEditPartial", new ManagerEditModelView() { 
                Email = employee.Email,
                NameSurname = employee.NameSurname,
                UserName = employee.UserName
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditManager(ManagerEditModelView model)
        {
            Employee employee = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {             
                employee.UserName = model.UserName;
                employee.Email = model.Email;
                employee.NameSurname = model.NameSurname;
                _db.Entry(employee).State = EntityState.Modified;
                await _db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            await SetViewBagRoles();
            return View("Index", new ManagerIndexModelView() { ManagerEditModel = model, IsEditInvalid = true, Employee = employee });
        }
        string GetRuRoleName(string role)
        {
            switch (role)
            {
                case "seller":
                    return "Менеджер по продажам";
                case "marketer":
                    return "Маркетолог";
                case "admin":
                    return "Системный администратор";
            }
            return null;
        }
        
        [Authorize]
        private async Task SetViewBagRoles()
        {
            Dictionary<string, string> rolesDic = new Dictionary<string, string>();
            var roles = await _db.Roles.Where(p => p.Name != "manager").ToArrayAsync();
            foreach (var item in roles)
                rolesDic.Add(item.Name, GetRuRoleName(item.Name));
            ViewBag.Roles = rolesDic;
        }
        
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModelView model)
        {
            Employee employee = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                if (!employee.OnTimePassword)
                    return BadRequest();
                var result = await _userManager.ChangePasswordAsync(employee, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    employee.OnTimePassword = false;
                    employee.PasswordState = PasswordStates.Normal;
                    _db.Entry(employee).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index");
                }               
            }
            await SetViewBagRoles();
            return View("Index", new ManagerIndexModelView() { Employee = employee, Model = model, IsModalInvalid = true }) ;
        }
        [Authorize]
        public IActionResult EditEmployee(long id)
        {
            Employee employee = _db.Employees.FirstOrDefault(e => e.Id == id);
            
                ManagerEditModelView model = new ManagerEditModelView()
                {
                    Id = id,
                    UserName = employee.UserName,
                    Email = employee.Email,
                    NameSurname = employee.NameSurname
                };
            

            return View(model);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> EditEmployee(ManagerEditModelView model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == model.Id);
                if (employee != null)
                {
                    employee.UserName = model.UserName;
                    employee.Email = model.Email;
                    employee.NameSurname = model.NameSurname;
                    _db.Entry(employee).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                }
            }
            return RedirectToAction("EditEmployee", "Manager", new {id = model.Id});
        }
        
    }
}