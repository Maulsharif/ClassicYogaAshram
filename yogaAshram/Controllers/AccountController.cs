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

namespace yogaAshram.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private YogaAshramContext _db;

        public AccountController(UserManager<Employee> userManager, SignInManager<Employee> signInManager, YogaAshramContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
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
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                    employee,
                    model.Password,
                    true,
                    false
                    );
                if (result.Succeeded)
                {
                    if(User.IsInRole("chief"))
                        return RedirectToAction("Details", "Chief");
                }
                ModelState.AddModelError("", "Не корректный пароль и(или) аутентификатор");
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Home");
        }
    }
}