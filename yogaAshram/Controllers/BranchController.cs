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
        public async Task<IActionResult> CreateBranch(string name, string info, string address)
        {
            Branch branch = new Branch()
            {
                Name = name,
                Info = info,
                Address = address
            };
            _db.Entry(branch).State = EntityState.Added;
            await _db.SaveChangesAsync();
            return RedirectToAction("Index", "Chief");
        }
    }
}