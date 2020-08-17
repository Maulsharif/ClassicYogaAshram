using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;

namespace yogaAshram.Controllers
{
    public class PaymentsController : Controller
    {
        private readonly YogaAshramContext _db;

        public PaymentsController(YogaAshramContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> GetCreateModalAjax(long clientId)
        {
            if (!_db.Clients.Any(p => p.Id == clientId))
                return NotFound();
            PaymentCreateModelView model = new PaymentCreateModelView{ ClientId = clientId };
            ViewBag.Memberships = await _db.Memberships.ToArrayAsync();
            return PartialView("PartialViews/CreatePartial", model);
        }
        
    }
}