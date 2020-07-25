using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;

namespace yogaAshram.Controllers
{
    [Authorize(Roles = "chief")]
    public class ChiefController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;

        public ChiefController(UserManager<Employee> userManager, SignInManager<Employee> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult CreateEmployee()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<IActionResult> CreateEmployee(AccountCreateModelView model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = new Employee()
                {
                    UserName = model.UserName,
                    Email = model.Email
                };
                var result = await _userManager.CreateAsync(employee, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(employee, false);
                    return RedirectToAction("Details");
                }
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }
    }
}