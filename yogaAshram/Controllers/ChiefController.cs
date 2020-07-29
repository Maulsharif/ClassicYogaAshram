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
using yogaAshram.Services;

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
            return View(new ChiefIndexModelView() { Employee = empl });
        }
        [HttpPost]    
        public async Task<IActionResult> CreateEmployee(string nameSurname, string userName, string email, string role)
        {
            if (String.IsNullOrEmpty(nameSurname) || !_db.Roles.Any(p => p.Name == role))
                return BadRequest();
            Employee employee = new Employee()
            {
                UserName = userName,
                Email = email,
                NameSurname = nameSurname
            };
            string newPsw = PasswordGenerator.Generate();
            var result = await _userManager.CreateAsync(employee, newPsw);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(employee, role);
                await EmailService.SendPassword(email, newPsw, Url.Action("Login", "Account", null, Request.Scheme));
                return Json("true");
            }
            string errors = "";
            foreach (var error in result.Errors)
                errors += error.Description;
            return Json(errors);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordModelView model)
        {
            Employee employee = await _userManager.GetUserAsync(User);
            if (ModelState.IsValid)
            {
                var result = await _userManager.ChangePasswordAsync(employee, model.CurrentPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    if (employee.OnTimePassword)
                    {
                        employee.OnTimePassword = false;
                        employee.PasswordState = PasswordStates.Normal;
                        _db.Entry(employee).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                    return RedirectToAction("Index");
                }               
            }
            Dictionary<string, string> rolesDic = new Dictionary<string, string>();
            var roles = await _db.Roles.ToArrayAsync();
            foreach (var item in roles)
                rolesDic.Add(item.Name, GetRuRoleName(item.Name));
            ViewBag.Roles = rolesDic;
            return View("../Chief/Index", new ChiefIndexModelView() { Employee = employee, Model = model, IsModalInvalid = true }) ;
        }
    }
}