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
        [HttpPost]
        public async Task<bool> ResetPasswordAjax()
        {
            Employee employee = await _userManager.GetUserAsync(User);
            string psw = PasswordGenerator.Generate();
            string code = await _userManager.GeneratePasswordResetTokenAsync(employee);
            var result = await _userManager.ResetPasswordAsync(employee, code, psw);
 //           await EmailService.SendPassword(employee.Email, psw, Url.Action());
            employee.OnTimePassword = true;
            _db.Entry(employee).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return result.Succeeded;
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
        public async Task<IActionResult> CreateEmployee(string nameSurname, string userName, string email)
        {
            if (String.IsNullOrEmpty(nameSurname))
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
            Dictionary<string, string> rolesDic = new Dictionary<string, string>();
            var roles = await _db.Roles.ToArrayAsync();
            foreach (var item in roles)
                rolesDic.Add(item.Name, GetRuRoleName(item.Name));
            ViewBag.Roles = rolesDic;
            return View("../Chief/Index", new ChiefIndexModelView() { Employee = employee, Model = model, IsModalInvalid = true }) ;
        }
    }
}