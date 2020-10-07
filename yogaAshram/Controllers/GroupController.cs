using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;
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
       
    
        
        
        [Authorize]
        [Breadcrumb("Добавить группу", FromAction = "Index", FromController = typeof(ChiefController))]
        public IActionResult CreateGroup()
        {
            ViewBag.Coaches = _db.Employees.Where(p => p.Role == "coach");
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
                CoachId = model.CoachId,
                BranchId = model.BranchId,
                CreatorId = GetUserId.GetCurrentUserId(this.HttpContext)
            };

            _db.Entry(group).State = EntityState.Added;
            await _db.SaveChangesAsync();
            
            if (User.IsInRole("chief"))
                return RedirectToAction("Index", "Chief");
           
            return RedirectToAction("Index", "Manager");
        }
       
        
        [Authorize]
        
        public async Task<IActionResult> History(long ?branchId, long groupId, int month, int year)
        {
            if (User.IsInRole("admin"))
            {  Employee user=await _userManager.GetUserAsync(User);
                branchId = _db.Branches.FirstOrDefault(p=>p.AdminId==user.Id).Id;
            }
            
          
            ViewBag.Groups = _db.Groups.Where(p => p.Id == branchId);
            
            
            List<List<Attendance>> res=new List<List<Attendance>>();
            List< Attendance> attendance =
                _db.Attendances.Where(p=>p.GroupId==groupId && p.ClientsMembership.DateOfPurchase.Month==month && p.ClientsMembership.DateOfPurchase.Year==year).ToList();
            List<long?> Ids=attendance.Select(att => att.ClientsMembershipId).Distinct().ToList();
            for (int i = 0; i < Ids.Count; i++)
            {
                res.Add(new List<Attendance>());
            }
            
            for (int i = 0; i < attendance.Count; i++)
            {
                for (int j = 0; j < Ids.Count; j++)
                {
                     if(attendance[i].ClientsMembershipId==Ids[j])
                         res[j].Add(attendance[i]);
                }
                
               
            }

            res.Sort((a,b) => b.Count - a.Count);
            Console.WriteLine(res.Count);
          
        
            return View(res);
        }
        
        
        
       
    }
}