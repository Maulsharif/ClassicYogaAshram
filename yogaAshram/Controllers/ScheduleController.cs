using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using yogaAshram.Models;
using yogaAshram.Models.ModelViews;

namespace yogaAshram.Controllers
{
    public class ScheduleController : Controller
    {
        private YogaAshramContext _db;

        public ScheduleController(YogaAshramContext db)
        {
            _db = db;
        }

        // GET
        public IActionResult Index(int? month, long? branchId)
        {
            if (_db.CalendarEvents != null || branchId != null) 
                ViewBag.Events = _db.CalendarEvents
                .Where(c => c.BranchId == branchId)
                .ToList();
            ViewBag.Groups = _db.Groups.Where(g => g.BranchId == branchId).ToList();
            List<Schedule> schedules = _db.Schedules.Where(s => s.BranchId == branchId).ToList();
            long[] groupIdArray = new long[schedules.Count];
            for (int i = 0; i < schedules.Count; i++)
            {
                groupIdArray[i] = schedules[i].GroupId;
            }

            ViewBag.BranchId = branchId;
            ViewBag.GroupIdArray = String.Join(" ", groupIdArray);
            
            DateTime dateTime = DateTime.Today;
            if (month != null)
                dateTime = new DateTime(dateTime.Year, Convert.ToInt32(month), 1);
            return View(dateTime);
        }

        public IActionResult Group(long groupId)
        {
            Schedule schedule = _db.Schedules.FirstOrDefault(g => g.GroupId == groupId);
            if (schedule != null)
            {
                ViewBag.DaysArray =  string.Join(",", schedule.DayOfWeeksString);
                
                 return View(schedule);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Create(TimeSpan scheduleTime, TimeSpan scheduleFinishTime, long groupId,
             string color, string dayOfWeeks)
        {
            List<string> dayOfWeekFromString = dayOfWeeks.Split(',').ToList();
            DayOfWeek[] days = new DayOfWeek[dayOfWeekFromString.Count];
            for (int i = 0; i < dayOfWeekFromString.Count; i++)
            {
                days[i] = DayOfWeekEn(dayOfWeekFromString[i]);
            }
         
            Group group = _db.Groups.FirstOrDefault(g => g.Id == groupId);

            if (group != null)
            {
                Schedule schedule = new Schedule()
                {
                    BranchId = group.BranchId,
                    GroupId = groupId,
                    StartTime = scheduleTime,
                    FinishTime = scheduleFinishTime,
                    DayOfWeeksString = dayOfWeekFromString
                };

                _db.Entry(schedule).State = EntityState.Added;
                foreach (var day in days)
                {
                    CalendarEvent calendarEvent = new CalendarEvent()
                    {
                        DayOfWeek = day,
                        Group = group.Name,
                        TimeStart = scheduleTime,
                        TimeFinish = scheduleFinishTime,
                        Type = SelectBootstrapColor(color),
                        BranchId = group.BranchId,
                        GroupId = group.Id,
                        Action = $"/Schedule/Group/?groupId={group.Id}"
                    };
                    List<CalendarEvent> events = _db.CalendarEvents.ToList();
                    foreach (var t in events)
                    {
                        if(t.BranchId == calendarEvent.BranchId && 
                           t.GroupId != calendarEvent.GroupId && 
                           t.DayOfWeek == calendarEvent.DayOfWeek)
                        {
                            if (t.TimeStart < calendarEvent.TimeStart && calendarEvent.TimeStart < t.TimeFinish ||
                                t.TimeStart < calendarEvent.TimeFinish && calendarEvent.TimeFinish < t.TimeFinish)
                            {
                                
                                return Content("errorTime");
                            }
                        }
                    }
                    _db.Entry(calendarEvent).State = EntityState.Added;
                }
            }
            else
                return NotFound();
            
            await _db.SaveChangesAsync();

            return Content("success");
        }
        
        [HttpPost]
        public async Task<IActionResult> Edit(TimeSpan scheduleTime, TimeSpan scheduleFinishTime, long groupId,
            string color, string dayOfWeeks)
        {
            List<string> dayOfWeekFromString = dayOfWeeks.Split(',').ToList();
            DayOfWeek[] days = new DayOfWeek[dayOfWeekFromString.Count];
            for (int i = 0; i < dayOfWeekFromString.Count; i++)
            {
                days[i] = DayOfWeekEn(dayOfWeekFromString[i]);
            }
            
            Schedule schedule = _db.Schedules.FirstOrDefault(s => s.GroupId == groupId);
            if (schedule != null)
            {
                schedule.DayOfWeeksString.Clear();
                schedule.DayOfWeeksString.AddRange(dayOfWeekFromString);
                schedule.StartTime = scheduleTime;
                schedule.FinishTime = scheduleFinishTime;
                
                _db.Entry(schedule).State = EntityState.Modified;
                
                foreach (var calendar in _db.CalendarEvents.Where(c => c.GroupId == schedule.GroupId))
                {
                    _db.Entry(calendar).State = EntityState.Deleted;
                }

                foreach (var day in days)
                {
                    CalendarEvent calendarEvent = new CalendarEvent()
                    {
                        DayOfWeek = day,
                        TimeStart = scheduleTime,
                        TimeFinish = scheduleFinishTime,
                        Group = schedule.Group.Name,
                        GroupId = schedule.GroupId,
                        BranchId = schedule.BranchId,
                        Type = SelectBootstrapColor(color),
                        Action = $"/Schedule/Group/?groupId={schedule.GroupId}"
                    };
                    List<CalendarEvent> events = _db.CalendarEvents.ToList();
                    foreach (var t in events)
                    {
                        if(t.BranchId == calendarEvent.BranchId && 
                           t.GroupId != calendarEvent.GroupId && 
                           t.DayOfWeek == calendarEvent.DayOfWeek)
                        {
                            if (t.TimeStart < calendarEvent.TimeStart && calendarEvent.TimeStart < t.TimeFinish ||
                                t.TimeStart < calendarEvent.TimeFinish && calendarEvent.TimeFinish < t.TimeFinish)
                            {
                                return Content("errorTime");
                            }
                        }
                    }
                    _db.Entry(calendarEvent).State = EntityState.Added;
                }

                await _db.SaveChangesAsync();
                return Content("success");
            }

            return NotFound();

        }

        public string SelectBootstrapColor(string colorBootstrap)
        {
            List<string> colors = new List<string> {"primary", "success", "danger", "warning", "info", "dark"};
            string color = colors.FirstOrDefault(c => c.Contains(colorBootstrap));
            return color;
        }

        private DayOfWeek DayOfWeekEn(string dayOfWeekRus)
        {
            switch (dayOfWeekRus)
            {
                case ("Воскресенье"):
                    return DayOfWeek.Sunday;
                case ("Понедельник"):
                    return DayOfWeek.Monday;
                case ("Вторник"):
                    return DayOfWeek.Tuesday;
                case ("Среда"):
                    return DayOfWeek.Wednesday;
                case ("Четверг"):
                    return DayOfWeek.Thursday;
                case ("Пятница"):
                    return DayOfWeek.Friday;
                case ("Суббота"):
                    return DayOfWeek.Saturday;
            }

            return DayOfWeek.Monday;
        }
    }
}



