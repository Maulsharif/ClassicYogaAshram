using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using yogaAshram.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using yogaAshram.Models.ModelViews;
using Microsoft.EntityFrameworkCore;
using yogaAshram.Services;

namespace yogaAshram.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private YogaAshramContext _db;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<Employee> userManager, SignInManager<Employee> signInManager, YogaAshramContext db, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
            _roleManager = roleManager;
        }        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AccountLoginModelView model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = await _userManager.FindByEmailAsync(model.Authentificator);
                if (employee is null)
                    employee = await _userManager.FindByNameAsync(model.Authentificator);
                if(employee.OnTimePassword)
                {
                    if (employee.PasswordState == PasswordStates.DisposableUsed)
                    {
                        ModelState.AddModelError("", "Одноразовый пароль уже был использован для входа");
                        return View(model);
                    }
                    else
                    {
                        employee.PasswordState = PasswordStates.DisposableUsed;
                        _db.Entry(employee).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                    }
                }
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                    employee,
                    model.Password,
                    true,
                    false
                    );
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Employees");
                }
                ModelState.AddModelError("", "Не корректный пароль и(или) аутентификатор");
            }
            return View(model);
        }
        [HttpPost]
        public async Task<bool> ResetPasswordAjax(string authentificator)
        {
            Employee employee = await _userManager.FindByEmailAsync(authentificator);
            if (employee is null)
                employee = await _userManager.FindByNameAsync(authentificator);
            if (employee is null)
                return false;
            string psw = PasswordGenerator.Generate();
            string code = await _userManager.GeneratePasswordResetTokenAsync(employee);
            var result = await _userManager.ResetPasswordAsync(employee, code, psw);
            await EmailService.SendPassword(employee.Email, psw, Url.Action());
            employee.OnTimePassword = true;
            _db.Entry(employee).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return result.Succeeded;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}