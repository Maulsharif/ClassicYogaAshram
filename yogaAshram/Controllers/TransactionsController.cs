using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yogaAshram.Models;
using yogaAshram.Services;

namespace yogaAshram.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly YogaAshramContext _db;
        private readonly UserManager<Employee> _userManager;

        public TransactionsController(UserManager<Employee> userManager, YogaAshramContext db)
        {
            _userManager = userManager;
            _db = db;
        }


        // GET
        public IActionResult Index(long branchId)
        {
            List<Payment> payments = _db.Payments.Where(p => p.BranchId == branchId).ToList();
            List<Withdrawal> withdrawals = _db.Withdrawals.Where(p => p.BranchId == branchId).ToList();
            
            return View();
        }
        
        public IActionResult Withdraw(long branchId)
        {
           //CurrentSum cs= _db.CurrentSums.FirstOrDefault(p => p.BranchId == branchId);
           //ViewBag.CurrentSum = cs;

            return View(new Withdrawal(){BranchId = branchId});
        }
        [HttpPost]
        public async Task<IActionResult> Withdraw(Withdrawal model)
        {
            CurrentSum cs= _db.CurrentSums.FirstOrDefault(p => p.BranchId == model.BranchId);
            if (ModelState.IsValid)
            {
                if (model.Sum<= cs.Total)
                {
                    model.CreatorId = GetUserId.GetCurrentUserId(HttpContext);
                    model.Date = DateTime.Now;
                    _db.Entry(model).State = EntityState.Added;
                    cs.CashSum -= model.Sum;
                    _db.Entry(cs).State = EntityState.Modified;
                    await _db.SaveChangesAsync();
                    return RedirectToAction("Index", new {branchId = model.BranchId});
                  
                }
                ModelState.AddModelError("Sum", "Недастаточно средств на счете");
            }
           
            return View(model );


        }

        
    }
}