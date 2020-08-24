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

        public MembershipController(YogaAshramContext db, UserManager<Employee> userManager)
        {
            _db = db;
            _userManager = userManager;
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

       
      
    }
}