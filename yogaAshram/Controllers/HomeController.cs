using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using yogaAshram.Models;
using yogaAshram.Services;

namespace yogaAshram.Controllers
{
    public class HomeController : Controller
    {
        private readonly YogaAshramContext _db;
        private readonly UserManager<Employee> _userManager;

        public HomeController(YogaAshramContext db, UserManager<Employee> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        // GET
        [DefaultBreadcrumb("Главная")]
        public IActionResult Index()
        {
            List<Payment> payments = _db.Payments
                .Where(c => c.ClientsMembership.ClientId == 10 && c.CateringDate.Date.Month == DateTime.Now.Month ).ToList();
         
            return View();
        }
    }
}