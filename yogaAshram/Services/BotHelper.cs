using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using yogaAshram.Models;

namespace yogaAshram.Services
{
    public class BotHelper
    {
        private readonly YogaAshramContext _db;

        public BotHelper(YogaAshramContext db)
        {
            _db = db;
        }

        public List<Attendance> GetListClents()
        {
           List<Attendance>clients= _db.Attendances.Where(p => p.Date == DateTime.Today && p.IsChecked == false).ToList();
           if (clients.Count > 0)
               return clients;
           else
           {
               return null;
           }

        }
    }
}