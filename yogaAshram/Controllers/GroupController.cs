using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yogaAshram.Models;

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
    }
}