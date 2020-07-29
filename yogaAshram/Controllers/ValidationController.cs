using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using yogaAshram.Models;

namespace yogaAshram.Controllers
{
    public class ValidationController : Controller
    {
        private YogaAshramContext _db;
        private readonly UserManager<Employee> _userManager;
        public ValidationController(YogaAshramContext db, UserManager<Employee> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        [Authorize]
        public async Task<bool> CheckPassword(string currentPassword)
        { 
            Employee empl = await _userManager.GetUserAsync(User);           
            return await _userManager.CheckPasswordAsync(empl, currentPassword);
        }
        public bool CheckRoleExistence(string role)
        {
            return _db.Roles.Any(p => p.Name == role);
        }
        public bool CheckUserNameCreate(string userName)
        {
            return !_db.Employees.Any(p => p.UserName == userName);
        }
        public bool CheckEmailCreate(string email)
        {
            return !_db.Employees.Any(p => p.Email == email);
        }
        public bool CheckEmailExist(string email)
        {
            return _db.Employees.Any(p => p.Email == email);
        }
        public bool CheckAuthValid(string authentificator)
        {
            if (_db.Employees.Any(p => p.UserName == authentificator))
                return true;
            return _db.Employees.Any(p => p.Email == authentificator);
        }
    }
}