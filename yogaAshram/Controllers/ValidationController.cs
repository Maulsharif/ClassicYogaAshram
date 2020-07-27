using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using yogaAshram.Models;

namespace yogaAshram.Controllers
{
    public class ValidationController : Controller
    {
        private YogaAshramContext _db;

        public ValidationController(YogaAshramContext db)
        {
            _db = db;
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
        public bool CheckAuthValid(string authentificator)
        {
            if (_db.Employees.Any(p => p.UserName == authentificator))
                return true;
            return _db.Employees.Any(p => p.Email == authentificator);
        }
    }
}