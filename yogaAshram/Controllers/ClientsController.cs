using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;
using yogaAshram.Services;
using State = yogaAshram.Models.State;

namespace yogaAshram.Controllers
{
    public class ClientsController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly YogaAshramContext _db;

        public ClientsController(UserManager<Employee> userManager, SignInManager<Employee> signInManager, YogaAshramContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }
        
        // [Authorize]
        // public IActionResult CreateClients(long branchId)
        // {
        //     
        //     ViewBag.Groups = _db.Groups.ToList().Where(b=>b.BranchId==branchId);
        //     return View();
        // }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateClients(Schedule schedule)
        {
            Client client = new Client
             {
                 NameSurname = schedule.ClientsCreateModelView.NameSurname,
                 PhoneNumber = schedule.ClientsCreateModelView.PhoneNumber,
                ClientType = schedule.ClientsCreateModelView.ClientType,
                 GroupId = schedule.ClientsCreateModelView.GroupId,
                 LessonNumbers =schedule.ClientsCreateModelView.LessonNumbers,
                 Comment =schedule.ClientsCreateModelView.Comment,
                 CreatorId = GetUserId.GetCurrentUserId(this.HttpContext)
             }; 
             _db.Entry(client).State = EntityState.Added;
             await _db.SaveChangesAsync();  
             long ClientId = client.Id;
             if (schedule.ClientsCreateModelView.LessonNumbers == 3)
             {
                List<DateTime>dates=TwoTimesTrial(schedule.ClientsCreateModelView.GroupId, schedule.ClientsCreateModelView.StartDate);
                foreach (var v in dates)
                {
                    Console.WriteLine(v);
                }
             }
             else
             {
                 TrialUsers trialUsers = new TrialUsers
                 {
                     GroupId = schedule.ClientsCreateModelView.GroupId,
                     ClientId = ClientId,
                     State = State.willAttend,
                     Color = "grey",
                     LessonTime = schedule.ClientsCreateModelView.StartDate
                 };
                 _db.Entry(trialUsers).State = EntityState.Added;
             }
            
             await _db.SaveChangesAsync();
            return RedirectToAction("Trials", "Clients");
        }


        

        public IActionResult Trials(DateTime time, long branchId)
        {
            ViewBag.BranchId = branchId;
            if(time<DateTime.Today)
                time=DateTime.Today;
            List<TrialUsers> users= _db.TrialUserses.Where(p => p.LessonTime.Date == time.Date).ToList();
             
            
            return View(users.Where(p => p.Group.BranchId == branchId).ToList());
        }

        [HttpGet]
        public IActionResult CheckAttendanceTrial(long groupId, long clientId )
        {
            TrialUsers user = _db.TrialUserses.FirstOrDefault(u => u.ClientId == clientId);
            
            
            List<TrialUsers> clients = _db.TrialUserses.Where(p => p.GroupId == groupId && p.LessonTime==user.LessonTime).ToList();
             
        
            return View(clients);
        }
        
        //Отметка пробников
        [HttpPost]
        public async Task<IActionResult> CheckAttendanceTrial(long[] arrayOfCustomerID, int []arrayOfState ) 
        {
            
            List<TrialCheckModel> models=new List<TrialCheckModel>();
            for (int i = 0; i < arrayOfCustomerID.Length; i++)
            {
                models.Add(new TrialCheckModel(arrayOfCustomerID[i],arrayOfState[i]));
            }

            List<TrialUsers> users = _db.TrialUserses.ToList();
            for (int i = 0; i < users.Count; i++)
            {
                for (int j = 0; j < models.Count; j++)
                {
                    if (users[i].Id == models[j].Id)
                    {
                        users[i].State = (State) models[j].State;
                        if (users[i].State == State.attended)
                        {
                            users[i].Color = "yellow";
                        }
                        else if (users[i].State==State.notattended)
                        {
                             users[i].Color = "red";
                        }
                        users[i].IsChecked = true;
                    }
                }
             
                _db.Entry(users[i]).State = EntityState.Modified;
            }
           
            await _db.SaveChangesAsync();
            
            return RedirectToAction("Trials", "Clients");
        }

        //метод возвращающий две даты 
        
        public List<DateTime> TwoTimesTrial(long? groupId, DateTime firstTime)
        {
            List<CalendarEvent> calendarEvents = _db.CalendarEvents.Where(c => c.GroupId == groupId).ToList();
            DayOfWeek[] dayOfWeeks = new DayOfWeek[calendarEvents.Count];
            for (int i = 0; i < calendarEvents.Count; i++)
            {
                dayOfWeeks[i] = calendarEvents[i].DayOfWeek;
            }
            DateTime tenDays = firstTime.AddDays(10);
            
            List<DateTime> dates = Enumerable.Range(0, 1 + tenDays.Subtract(firstTime).Days)
                .Select(offset => firstTime.AddDays(offset))
                .Where(d => dayOfWeeks.Contains(d.DayOfWeek))
                .ToList();
            
            DateTime [] dateTimes = new DateTime[2];
    
            dateTimes[0] = dates[1];
            dateTimes[1] = dates[2];

            return dateTimes.ToList();
        }

        public IActionResult ClientInfo(long Id)
        {
            TrialUsers client = _db.TrialUserses.FirstOrDefault(p => p.ClientId == Id);
            return View(client);

        }
    }
}