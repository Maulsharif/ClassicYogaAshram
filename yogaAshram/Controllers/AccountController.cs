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
            await EmailService.SendPassword(employee.Email, psw, Url.Action("Login", "Account"));
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
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                        new {email = model.Email, token = token}, Request.Scheme);
                    string message = $"<p>Здравствуйте!</p><p>Вы запросили восстановление пароля.</p>" +
                                     $"<p>Пожалуйста, перейдите по этой</p>" +
                                     $" <a href=" + '"' + passwordResetLink  +'"' + ">ссылке</a>," +
                                     " придумайте и введите ваш новый пароль.</p>";
                    await EmailService.SendMessageAsync(model.Email, "Восстановление пароля", message);
                    return View("ForgotPasswordConfirmation");
                }
                return View("ForgotPasswordConfirmation");
            }
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if(token is null || email is null)
                ModelState.AddModelError("", "Невалидный токен");
            return View();
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                    if (result.Succeeded)
                    {
                        user.OnTimePassword = false;
                        user.PasswordState = PasswordStates.Normal;
                        _db.Entry(user).State = EntityState.Modified;
                        await _db.SaveChangesAsync();
                        return View("ResetPasswordConfirmation");
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }

                    return View(model);
                }
                return View("ResetPasswordConfirmation");
            }
            return View(model);
        }
    }
}