using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;
using yogaAshram.Services;

namespace yogaAshram.Controllers
{
    [Authorize(Roles = "admin,chief")]
    public class AdminController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly YogaAshramContext _db;

        public AdminController(SignInManager<Employee> signInManager, YogaAshramContext db,
            UserManager<Employee> userManager)
        {
            _signInManager = signInManager;
            _db = db;
            _userManager = userManager;
        }

        [Authorize ]

          public async Task<IActionResult> Index()
        {
            Employee empl = await _userManager.GetUserAsync(User);
            Branch branch = _db.Branches.FirstOrDefault(b => b.AdminId == empl.Id);
            List<Group> groups;
            if (branch != null) groups = _db.Groups.Where(p => p.BranchId == branch.Id).ToList();
            else groups = null;
            

            return View(new AdminIndexModel()
            {
                Employee = empl,
                Branch = branch,
                Groups = groups
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
             var roles = await _db.Roles.Where(p => p.Name != "admin").ToArrayAsync();
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
             return View("Index", new AdminIndexModel() { Employee = employee, Model = model, IsModalInvalid = true }) ;
         }
         
         public IActionResult Scheduele(int? month, long? branchId)
         {
             if (_db.CalendarEvents != null || branchId != null) 
                 ViewBag.Events = _db.CalendarEvents
                     .Where(c => c.BranchId == branchId)
                     .ToList();
             ViewBag.Groups = _db.Groups.Where(g => g.BranchId == branchId).ToList();
             List<Schedule> schedules = _db.Schedules.Where(s => s.BranchId == branchId).ToList();
             long[] groupIdArray = new long[schedules.Count];
             for (int i = 0; i < schedules.Count; i++)
             {
                 groupIdArray[i] = schedules[i].GroupId;
             }

             ViewBag.BranchId = branchId;
             ViewBag.GroupIdArray = String.Join(" ", groupIdArray);
            
             DateTime dateTime = DateTime.Today;
             if (month != null)
                 dateTime = new DateTime(dateTime.Year, Convert.ToInt32(month), 1);
             return View(dateTime);
         }
       
        
        
    }
}