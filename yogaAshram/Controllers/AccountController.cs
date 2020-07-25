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
        public string GeneratePassword()
        {
            string password = Guid.NewGuid().ToString();
            return password.Remove(' ');
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(employee, GeneratePassword());
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(employee, false);
                    return RedirectToAction("Details");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(employee);
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string authetificator, string password)
        {
            if (ModelState.IsValid)
            {
                Employee employee = await _userManager.FindByEmailAsync(authetificator);
                if (employee is null)
                    employee = await _userManager.FindByNameAsync(authetificator);
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(
                    employee,
                    password,
                    true,
                    false
                    );
                if (result.Succeeded)
                {
                    return RedirectToAction("Details");
                }
                ModelState.AddModelError("", "Не корректный пароль и(или) аутентификатор");
            }
            return View();
        }
    }
}