using System;
using System.Collections.Generic;
using System.Linq;
using yogaAshram.Models;

namespace yogaAshram.Services
{
    public class ClientServices
    {
        private readonly YogaAshramContext _db;

        public ClientServices(YogaAshramContext db)
        {
            _db = db;
        }

        public List<DateTime> DatesForAttendance(DateTime firstTime, long groupId, int attendanceDays)
        {
            List<CalendarEvent> calendarEvents = _db.CalendarEvents.Where(c => c.GroupId == groupId).ToList();
            DayOfWeek[] dayOfWeeks = new DayOfWeek[calendarEvents.Count];
            for (int i = 0; i < calendarEvents.Count; i++)
            {
                dayOfWeeks[i] = calendarEvents[i].DayOfWeek;
            }
            DateTime fortyDays = firstTime.AddDays(40);
            
            List<DateTime> dates = Enumerable.Range(0, 1 + fortyDays.Subtract(firstTime).Days)
                .Select(offset => firstTime.AddDays(offset))
                .Where(d => dayOfWeeks.Contains(d.DayOfWeek))
                .ToList();
            
            DateTime [] dateTimes = new DateTime[attendanceDays];
            for (int i = 0; i < attendanceDays; i++)
            {
                dateTimes[i] = dates[i];
            }
            
            return dateTimes.ToList();
        } 
        public List<DateTime> DatesSkipFirst(DateTime firstTime, long groupId, int attendanceDays)
        {
            List<CalendarEvent> calendarEvents = _db.CalendarEvents.Where(c => c.GroupId == groupId).ToList();
            DayOfWeek[] dayOfWeeks = new DayOfWeek[calendarEvents.Count];
            for (int i = 0; i < calendarEvents.Count; i++)
            {
                dayOfWeeks[i] = calendarEvents[i].DayOfWeek;
            }
            DateTime fortyDays = firstTime.AddDays(43);
            
            List<DateTime> dates = Enumerable.Range(0, 1 + fortyDays.Subtract(firstTime).Days)
                .Select(offset => firstTime.AddDays(offset))
                .Where(d => dayOfWeeks.Contains(d.DayOfWeek))
                .ToList();
            
            DateTime [] dateTimes = new DateTime[attendanceDays + 1];
            for (int i = 0; i < attendanceDays; i++)
            {
                dateTimes[i] = dates[i];
            }
            return dateTimes.ToList();
        } 
        public List<DateTime> TwoTimesTrial(long? groupId, DateTime firstTime)
        {
            List<CalendarEvent> calendarEvents = _db.CalendarEvents.Where(c => c.GroupId == groupId).ToList();
            DayOfWeek[] dayOfWeeks = new DayOfWeek[calendarEvents.Count];
            for (int i = 0; i < calendarEvents.Count; i++)
            {
                dayOfWeeks[i] = calendarEvents[i].DayOfWeek;
            }
            DateTime tenDays = firstTime.AddDays(10);
            
            List<DateTime> dates = Enumerable.Range(0, 1 + tenDays.Subtract(firstTime).Days)
                .Select(offset => firstTime.AddDays(offset))
                .Where(d => dayOfWeeks.Contains(d.DayOfWeek))
                .ToList();
            
            DateTime [] dateTimes = new DateTime[2];
    
            dateTimes[0] = dates[1];
            dateTimes[1] = dates[2];

            return dateTimes.ToList();
        }
    }
}