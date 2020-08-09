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
    [Authorize(Roles = "seller")]
    public class SellerController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly YogaAshramContext _db;
        private readonly SignInManager<Employee> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        public SellerController(UserManager<Employee> userManager, YogaAshramContext db, SignInManager<Employee> signInManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _db = db;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET
        public async Task<IActionResult> Index()
        {
            Employee empl = await _userManager.GetUserAsync(User);

            return View(new SellerIndexModel()
            {
                Employee = empl,
                Groups = _db.Groups.ToList()
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            Employee model = await _userManager.GetUserAsync(User);
                if (model == null)
                {
                    return NotFound();
                }
                return View(new EmployeeEditModelView { 
                    Email = model.Email,
                    NameSurname = model.NameSurname,
                    UserName = model.UserName});
            
        }
       
         [HttpPost]
        [ValidateAntiForgeryToken]
         public async Task<IActionResult> Edit( EmployeeEditModelView model)
         {
             Employee employee = await _userManager.GetUserAsync(User);
             if (ModelState.IsValid)
             { employee.UserName = model.UserName;
                employee.Email = model.Email;
                employee.NameSurname = model.NameSurname;
                 _db.Entry(employee).State = EntityState.Modified;
                 await _db.SaveChangesAsync();
                 return Json(new { isValid = true });
             }

             return Json(new
                 {isValid = false, html = RenderFromString.RenderRazorViewToString(this, "Edit", model)});
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
             var roles = await _db.Roles.Where(p => p.Name != "seller").ToArrayAsync();
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
             return View("Index", new SellerIndexModel() { Employee = employee, Model = model, IsModalInvalid = true }) ;
         }
         
         
         
         
         
         
         
         


    }

}
