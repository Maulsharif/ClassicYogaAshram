using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
                 CreatorId = GetUserId.GetCurrentUserId(this.HttpContext)
             }; 
            Employee employee =
                _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
            if(schedule.ClientsCreateModelView.Comment != null)
                client.Comments = new List<string> {$"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}"};
            _db.Entry(client).State = EntityState.Added;
             await _db.SaveChangesAsync();  
             long ClientId = client.Id;
             if (schedule.ClientsCreateModelView.LessonNumbers == 3)
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
                List<DateTime>dates=TwoTimesTrial(schedule.ClientsCreateModelView.GroupId, schedule.ClientsCreateModelView.StartDate);

                for (int i = 0; i < 2; i++)
                {
                     trialUsers = new TrialUsers
                    {
                        GroupId = schedule.ClientsCreateModelView.GroupId,
                        ClientId = ClientId,
                        State = State.willAttend,
                        Color = "grey",
                        LessonTime = dates[i]
                    };
                     _db.Entry(trialUsers).State = EntityState.Added; 
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
                 return RedirectToAction("Trials", "Clients",new {branchId= schedule.BranchId});
        }


        

        public IActionResult Trials(DateTime time, long branchId)
        {
            ViewBag.BranchId = branchId;
            if(time<DateTime.Today)
                return View(_db.TrialUserses.Where(p => p.Group.BranchId == branchId).OrderBy(p=>p.LessonTime).ToList());
            List<TrialUsers> users= _db.TrialUserses.Where(p => p.LessonTime.Date == time.Date).ToList();
            
            return View(users.Where(p => p.Group.BranchId == branchId).ToList());
        }

        [HttpGet]
        public IActionResult CheckAttendanceTrial(long groupId, long clientId, long branchId )
        {
            
            TrialUsers user = _db.TrialUserses.FirstOrDefault(u => u.ClientId == clientId);
            
            
            List<TrialUsers> clients = _db.TrialUserses.Where(p => p.GroupId == groupId && p.LessonTime==user.LessonTime).ToList();

            ViewBag.BranchIdHidden = branchId;
            return View(clients);
        }
        
        //Отметка пробников
        [HttpPost]
        public async Task<IActionResult> CheckAttendanceTrial(long[] arrayOfCustomerID, int []arrayOfState, long HbranchId ) 
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
                           // users[i].Client.LessonNumbers -= 1;
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
        
           return RedirectToAction("Trials", "Clients",new {branchId= HbranchId});
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
            ViewBag.Lessons = _db.TrialUserses.Where(p => p.ClientId == Id);
            TrialUsers client = _db.TrialUserses.FirstOrDefault(p => p.ClientId == Id);
            return View(client);

        }

        [HttpPost]
        public async Task<IActionResult> NewClientRegister(Schedule schedule)
        {
            if (ModelState.IsValid)
            {
                Client client = new Client()
                {
                    NameSurname = schedule.ClientsCreateModelView.NameSurname,
                    PhoneNumber = schedule.ClientsCreateModelView.PhoneNumber,
                    DateOfBirth = schedule.ClientsCreateModelView.DateOfBirth,
                    Email = schedule.ClientsCreateModelView.Email,
                    Address = schedule.ClientsCreateModelView.Address,
                    WorkPlace = schedule.ClientsCreateModelView.WorkPlace,
                    Sickness = schedule.ClientsCreateModelView.Sickness,
                    Source = schedule.ClientsCreateModelView.Source,
                    Paid = Paid.Не_оплачено,
                    WhatsAppGroup = WhatsAppGroup.Не_состоит_в_группе,
                    Contract = Contract.Нет_договора,
                    GroupId = schedule.ClientsCreateModelView.GroupId,
                    MembershipId = schedule.ClientsCreateModelView.MembershipId,
                    ClientType = ClientType.AreEngaged,
                    CreatorId = GetUserId.GetCurrentUserId(this.HttpContext)
                };
                Employee employee =
                    _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
                if(schedule.ClientsCreateModelView.Comment != null)
                  client.Comments = new List<string> {$"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}"};
                _db.Entry(client).State = EntityState.Added;
                Group group = _db.Groups.FirstOrDefault(g => g.Id == schedule.ClientsCreateModelView.GroupId);
                if(group != null && group.Clients.Count == 0)
                    group.Clients = new List<Client>()
                    {
                        client
                    };
                else
                    group?.Clients.Add(client);
                

                _db.Entry(group).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            
            return RedirectToAction("RegularClients", "Clients");
        }
        [HttpPost]
        public async Task<IActionResult> OldClientRegister(Schedule schedule, long clientId)
        {
            if (ModelState.IsValid)
            {
                Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);

                Debug.Assert(client != null, nameof(client) + " != null");
                client.NameSurname = schedule.ClientsCreateModelView.NameSurname;
                client.PhoneNumber = schedule.ClientsCreateModelView.PhoneNumber;
                client.DateOfBirth = schedule.ClientsCreateModelView.DateOfBirth;
                client.Email = schedule.ClientsCreateModelView.Email;
                client.Address = schedule.ClientsCreateModelView.Address;
                client.WorkPlace = schedule.ClientsCreateModelView.WorkPlace;
                client.Sickness = schedule.ClientsCreateModelView.Sickness;
                client.Source = schedule.ClientsCreateModelView.Source;
                Employee employee =
                    _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
                if (client.Comments.Count == 0 && schedule.ClientsCreateModelView.Comment != null) 
                    client.Comments = new List<string> {$"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}"};
                else client.Comments.Add($"{employee?.UserName}: {schedule.ClientsCreateModelView.Comment}, {DateTime.Now:dd.MM.yyyy}");
                client.Paid = Paid.Не_оплачено;
                client.WhatsAppGroup = WhatsAppGroup.Не_состоит_в_группе;
                client.Contract = Contract.Нет_договора;
                client.GroupId = schedule.ClientsCreateModelView.GroupId;
                client.MembershipId = schedule.ClientsCreateModelView.MembershipId;
                client.ClientType = ClientType.AreEngaged;
                client.CreatorId = GetUserId.GetCurrentUserId(this.HttpContext);
                

                _db.Entry(client).State = EntityState.Modified;
                Group group = _db.Groups.FirstOrDefault(g => g.Id == schedule.ClientsCreateModelView.GroupId);
                if(group != null && group.Clients.Count == 0)
                    group.Clients = new List<Client>()
                    {
                        client
                    };
                else
                    group?.Clients.Add(client);
                

                _db.Entry(group).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            
            return RedirectToAction("RegularClients", "Clients");
        }
        
        [Authorize]
        public IActionResult RegularClients()
        {
            List<Client> clients = _db.Clients.Where(c => c.ClientType == ClientType.AreEngaged).ToList();
            
            return View(clients);
        }

        [HttpPost]
        public async Task<IActionResult> MakeClientUnActive(long? clientId)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client != null) client.ClientType = ClientType.NotEngaged;
            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("RegularClients");
        }
        [HttpPost]
        public async Task<IActionResult> MakeClientJoinInWhatsAppGroup(long? clientId)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client != null) client.WhatsAppGroup = WhatsAppGroup.Состоит_в_группе;
            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("RegularClients");
        }
        [HttpPost]
        public async Task<IActionResult> ClientSignedContract(long? clientId)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            if (client != null) client.Contract = Contract.Есть_договор;
            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("RegularClients");
        }
        [HttpPost]
        public async Task<IActionResult> Comment(long? clientId, string comment)
        {
            Client client = _db.Clients.FirstOrDefault(c => c.Id == clientId);
            Employee employee =
                _db.Employees.FirstOrDefault(e => e.Id == GetUserId.GetCurrentUserId(this.HttpContext));
            if (client != null && (client.Comments.Count == 0 && comment != null)) 
                client.Comments = new List<string> {$"{employee?.UserName}: {comment}, {DateTime.Now:dd.MM.yyyy}"};
            else
                client?.Comments.Add(
                    $"{employee?.UserName}: {comment}, {DateTime.Now:dd.MM.yyyy}");


            _db.Entry(client).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return RedirectToAction("RegularClients");
        }
    }
}