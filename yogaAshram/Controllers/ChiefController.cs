using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;

namespace yogaAshram.Controllers
{
    [Authorize(Roles = "chief")]
    public class ChiefController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly YogaAshramContext _db;

        public ChiefController(UserManager<Employee> userManager, SignInManager<Employee> signInManager, YogaAshramContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }
        string GetRuRoleName(string role)
        {
            switch (role)
            {
                case "manager":
                    return "Старший менеджнер";
                case "seller":
                    return "Менеджер по продажам";
                case "marketer":
                    return "Маркетолог";
                case "admin":
                    return "Системный администратор";
            }
            return null;
        }
        public async Task<IActionResult> Index()
        {
            Employee empl = await _userManager.GetUserAsync(User);
            Dictionary<string, string> rolesDic = new Dictionary<string, string>();
            var roles = await _db.Roles.ToArrayAsync();
            foreach (var item in roles)
                rolesDic.Add(item.Name, GetRuRoleName(item.Name));
            ViewBag.Roles = rolesDic;
            return View(empl);
        }
        [HttpPost]    
        public async Task<IActionResult> CreateEmployee(string nameSurname, string userName, string email, string password, string confirmPassword)
        {
            AccountCreateModelView model = new AccountCreateModelView()
            {
                NameSurname = nameSurname,
                UserName = userName,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };
                Employee employee = new Employee()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    NameSurname = model.NameSurname
                };
                var result = await _userManager.CreateAsync(employee, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(employee, false);
                    return Json("true");
                }
                string errors = "";
                foreach (var error in result.Errors)
                    errors += error.Description;
                return Json(errors);
        }
    }
}