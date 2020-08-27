using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
    public class MembershipController : Controller
    { 
        private readonly UserManager<Employee> _userManager;
        private readonly YogaAshramContext _db;
        private readonly PaymentsService _paymentsService;

        public MembershipController(YogaAshramContext db, UserManager<Employee> userManager, PaymentsService paymentsService)
        {
            _db = db;
            _userManager = userManager;
            _paymentsService = paymentsService;
        }

        // GET
        public IActionResult Index()
        {
            List<Membership> model = _db.Memberships.ToList(); 
            return View(model);
        }


        public async Task<IActionResult> AddOrEdit(long id = 0)
        {
          
            if (id == 0)
                return View(new Membership());
            else
            {
                var membershipModel = await _db.Memberships.FindAsync(id);
                if (membershipModel == null)
                {
                    return NotFound();
                }
                return View(membershipModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit(long id, Membership membershipModel)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    
                    _db.Entry(membershipModel).State = EntityState.Added;
                    await _db.SaveChangesAsync();

                }
                else
                {
                    try
                    {
                        _db.Update(membershipModel);
                        await   _db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (_db.Memberships.FirstOrDefault(p=>p.Id==id)==null)
                        { return NotFound(); }
                        else
                        { throw; }
                    }
                }
                return Json(new { isValid = true, html = Helper.RenderRazorViewToString(this, "_ViewAll", _db.Memberships.ToList()) });
            }
            return Json(new { isValid = false, html = Helper.RenderRazorViewToString(this, "AddOrEdit", membershipModel) });
        }
        [HttpGet]
        public async Task<IActionResult> GetExtendModalAjax(long clientId, string date)
        {
            Client client = await _db.Clients.FindAsync(clientId);
            if (client is null)
                return NotFound();
            ViewBag.Memberships = await _db.Memberships.ToArrayAsync();
            ViewBag.Groups = await _db.Groups.ToListAsync();
            ViewBag.Date = Convert.ToDateTime(date);
            return PartialView("PartialViews/ExtendModalPartial" ,new MembershipExtendModelView() { ClientId = client.Id, Client = client });
        }
        [HttpPost]
        public async Task<IActionResult> ExtendAjax(MembershipExtendModelView model)
        {
            if (ModelState.IsValid)
            {
                if (model.CardSum is null)
                    model.CardSum = 0;
                if (model.CashSum is null)
                    model.CashSum = 0;
                Client client = await _db.Clients.FirstOrDefaultAsync(p => p.Id == model.ClientId);
                if (client.Balance < 0 && -client.Balance > model.CashSum + model.CardSum)
                    return BadRequest();

                Schedule schedule = _db.Schedules.FirstOrDefault(s => s.GroupId == model.GroupId);

                Console.WriteLine(model.Date);
                Console.WriteLine(model.GroupId);
                Console.WriteLine(model.MembershipId);
                
                
                Membership membership = await _db.Memberships.FirstOrDefaultAsync(p => p.Id == model.MembershipId);
                client.MembershipId = membership.Id;
                client.GroupId = model.GroupId;
                client.Membership = membership;
                client.LessonNumbers = membership.AttendanceDays;
                ClientsMembership clientsMembership = new ClientsMembership()
                {
                    Id = _db.Memberships.Last().Id + 1,
                    Client = client,
                    ClientId = client.Id,
                    Membership = membership,
                    MembershipId = membership.Id,
                    DateOfPurchase = DateTime.Now
                };
                Employee employee = await _userManager.GetUserAsync(User);
                _db.Entry(clientsMembership).State = EntityState.Added;
                await _db.SaveChangesAsync();
                bool check = await _paymentsService.CreatePayment(model, clientsMembership, client, employee.Id);
                if (!check)
                    return BadRequest();
                return Json(true);
            }
            return BadRequest();
        }
    }   
}