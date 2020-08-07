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
        public IActionResult CreateClients()
        {
            ViewBag.Groups = _db.Groups.ToList();
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
                CreatorId = GetUserId.GetCurrentUserId(this.HttpContext)
            };
            
            _db.Entry(client).State = EntityState.Added;
            await _db.SaveChangesAsync();
            
            if (User.IsInRole("chief"))
                return RedirectToAction("Index", "Chief");
           
            return RedirectToAction("Index", "Admin");
        }
    }
}