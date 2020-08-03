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
        
        [Authorize]
        public IActionResult CreateGroup()
        {
            return View();
        }
        
        
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateGroup(CreateGroupModelView model)
        {
            Group group = new Group
            {
                Id = model.Id,
                Name =  model.Name,
                CoachName = model.CoachName,
                Branch = model.Branch,
                BranchId = model.BranchId,
                CreatorId = model.CreatorId,
                Employee = model.Employee
            };
            
            _db.Entry(group).State = EntityState.Added;
            await _db.SaveChangesAsync();
            
            return View(model);
        }
    }
}