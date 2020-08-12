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
        
        [Authorize]
        public IActionResult CreateClients(long branchId)
        {
            
            ViewBag.Groups = _db.Groups.ToList().Where(b=>b.BranchId==branchId);
            return View();
        }
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateClients(ClientsCreateModelView model)
        {
            
            Client client = new Client
            {
                NameSurname = model.NameSurname,
                PhoneNumber = model.PhoneNumber,
                ClientType = model.ClientType,
                GroupId = model.GroupId,
                LessonNumbers = model.LessonNumbers,
                Comment = model.Comment,
                CreatorId = GetUserId.GetCurrentUserId(this.HttpContext)
            }; 
            _db.Entry(client).State = EntityState.Added;
            await _db.SaveChangesAsync();  
            long ClientId = client.Id;
            if (model.LessonNumbers == 3)
            {
                //метод для 
            }
            else
            {  
                TrialUsers trialUsers = new TrialUsers
                         {
                             GroupId = model.GroupId,
                             ClientId = ClientId,
                             State = State.willAttend,
                             Color = "grey",
                             LessonTime = model.StartDate
                         };
                         _db.Entry(trialUsers).State = EntityState.Added;
                
            }

         
          
            await _db.SaveChangesAsync();
            return RedirectToAction("Trials", "Clients");
            
        }


        

        public IActionResult Trials(DateTime time)
        {
            if(time<DateTime.Today)
                time=DateTime.Today;
            return View(_db.TrialUserses.Where(p=>p.LessonTime.Date==time.Date).ToList());
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
                    }
                }

                _db.Entry(users[i]).State = EntityState.Modified;
            }
           
            await _db.SaveChangesAsync();
            
            return RedirectToAction("Trials", "Clients");
        }

        //метод возвращающий две даты 
        // public DateTime[] DateTimes(string days)
        // {
        //     
        // }


    }
}