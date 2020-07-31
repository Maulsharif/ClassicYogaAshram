using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yogaAshram.Models;

namespace yogaAshram.Controllers
{
    [Authorize(Roles = "chief")]
    public class BranchController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;
        private readonly YogaAshramContext _db;

        public BranchController(UserManager<Employee> userManager, SignInManager<Employee> signInManager, YogaAshramContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateBranch(string name, string info, string address, long? marketerId, long? sellerId)
        {
            Branch branch = new Branch()
            {
                Name = name,
                Info = info,
                Address = address,
                MarketerId = marketerId,
                SellerId = sellerId
            };
            _db.Entry(branch).State = EntityState.Added;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Chief");
        }
        [HttpPost]
        public async Task<IActionResult> Edit(long id, 
            string name, 
            string address,
            string info,
            long? sellerId,
            long? marketerId)
        {
            Branch branch = _db.Branches.FirstOrDefault(p => p.Id == id);
            if (branch != null)
            {
                if (name != null)
                    branch.Name = name;
                
                else if(address != null)
                    branch.Address = address;
                
                else if(info != null)
                    branch.Info = info;
                
                else if(sellerId != null)
                    branch.SellerId = sellerId;
                
                else if(marketerId != null)
                    branch.MarketerId = marketerId;

                _db.Entry(branch).State = EntityState.Modified;
                await _db.SaveChangesAsync();
            }
            return RedirectToAction("Index", "Chief");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            Branch branch = _db.Branches.FirstOrDefault(p => p.Id == id);
            _db.Entry(branch).State = EntityState.Deleted;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Chief");
        }
    }
}