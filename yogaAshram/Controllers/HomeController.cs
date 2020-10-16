using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
      
        // public IActionResult Index()
        // {
        //     // return View();
        // }
    }
}