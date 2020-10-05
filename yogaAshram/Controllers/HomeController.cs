using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using yogaAshram.Models;
using yogaAshram.Models.HelperModels;
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
            AttendanceHistoryShow list = new AttendanceHistoryShow();
        
            List<int> primeNumbers = new List<int>();
            primeNumbers.Add(1); // adding elements using add() method
            primeNumbers.Add(3);
            primeNumbers.Add(5);
            primeNumbers.Add(7);
            list.Days = primeNumbers;
            list.Name = "anna";
            list.Group = "Gh-1";
            
          
            return View(list);
        }
    }
}