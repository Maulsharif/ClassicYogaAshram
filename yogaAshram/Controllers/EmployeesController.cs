using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
         public IActionResult Search(string search)
                {
                    if (User.IsInRole("chief"))
                    {
                        if (string.IsNullOrEmpty(search))
                        {
                            ViewBag.Error = "Введите имя сотрудника для поиска";
                            return PartialView("PartialView", _db.Employees.Where(e => e.Id != GetUserId.GetCurrentUserId(this.HttpContext)).ToList());
                        }
                        search = search.ToUpper();
                        List<Employee> employees = _db.Employees
                            .Where(e => e.Id != GetUserId.GetCurrentUserId(this.HttpContext))
                            .Where(p => p.UserName.ToUpper().Contains(search)
                                        || p.NameSurname.ToUpper().Contains(search))
                            .ToList();
                        if (employees.Count == 0)
                        {
                            ViewBag.Error = "Совпадений не найдено";
                        }
                        return PartialView("PartialView", employees); 
                    } 
                    
                    if (string.IsNullOrEmpty(search))
                    {
                        ViewBag.Error = "Введите имя пользователя для поиска";
                        return PartialView("PartialViewManager", _db.ManagerEmployees
                            .Include(c => c.Employee)
                            .Include(c => c.Manager)
                            .ToList());
                    }
                    search = search.ToUpper();
                    List<ManagerEmployee> managerEmployees = _db.ManagerEmployees
                        .Include(c => c.Employee)
                        .Include(c => c.Manager)
                        .Where(p => p.Employee.NameSurname.ToUpper().Contains(search) 
                                    || p.Employee.UserName.ToUpper().Contains(search))
                        .ToList();
                    if (managerEmployees.Count == 0)
                    {
                        ViewBag.Error = "Совпадений не найдено";
                    }
                    return PartialView("PartialViewManager", managerEmployees);

                }
    }
}