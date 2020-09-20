using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Attributes;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;
using yogaAshram.Services;

namespace yogaAshram.Controllers
{ 
    
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
        [Breadcrumb("Личный кабинет директора")]
        public async Task<IActionResult> Index()
        {
            Employee empl = await _userManager.GetUserAsync(User);

            return View(new SellerIndexModel()
            {
                Employee = empl,
                Branches = _db.Branches.ToList(),
                Clients = _db.Clients.ToList()
            });
        }
        
        public IActionResult SchedulePage(int? month, long? branchId)
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

        
        [HttpPost]
        public async Task<IActionResult> WriteComment(long clientId, string sellerComment)
        {
            
            TrialUsers client = _db.TrialUserses.FirstOrDefault(c => c.Id == clientId);
            Employee employee =
                _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
          
                client.SellerComments = new List<Comment>
                {
                    new Comment()
                    {
                        Text = $"{employee?.UserName}: {sellerComment}, {DateTime.Now:dd.MM.yyyy}",
                        Reason = Reason.Другое,
                        ClientId = client.Id
                    }
                };
           
            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            
          return  RedirectToAction("ClientInfo", "Clients" ,new {id= client.ClientId });

        }
        












    }

}
