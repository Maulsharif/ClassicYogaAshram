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
    public class GroupController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly YogaAshramContext _db;
        
        public GroupController(UserManager<Employee> userManager, SignInManager<Employee> signInManager, YogaAshramContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }
       
        public IActionResult Index()
        {
            List<Schedule> schedules = _db.Schedules.ToList();
            return View(schedules);
        }
        
        
        [Authorize]
        public IActionResult CreateGroup()
        {
            ViewBag.Branches = _db.Branches.ToList();
            return View();
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateGroup(CreateGroupModelView model)
        {
            Group group = new Group
            {
                Name = model.Name,
                CoachName = model.CoachName,
                BranchId = model.BranchId,
                CreatorId = GetUserId.GetCurrentUserId(this.HttpContext)
            };

            _db.Entry(group).State = EntityState.Added;
            await _db.SaveChangesAsync();
            
            if (User.IsInRole("chief"))
                return RedirectToAction("Index", "Chief");
           
            return RedirectToAction("Index", "Manager");
        }
        [HttpGet]
        public IActionResult CreateScheduele()
        {
            ViewBag.Groups = _db.Groups.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateScheduele(Schedule model)
        {  ViewBag.Groups = _db.Groups.ToList();
            List<Schedule> sch = _db.Schedules.ToList();
            if (sch.Any(p => p.FromHours == model.FromHours && p.DayOfWeek == model.DayOfWeek))
            {
                ModelState.AddModelError(nameof(model.DayOfWeek), "Время уже занято!");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                _db.Entry(model).State = EntityState.Added;
                await _db.SaveChangesAsync();
                          
                return RedirectToAction("Index");
            }
            return View(model);
         
        }




        
    }
}