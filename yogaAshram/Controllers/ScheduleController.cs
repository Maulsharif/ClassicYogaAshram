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
        public IActionResult Index(int? month)
        {
            if (_db.CalendarEvents != null) ViewBag.Events = _db.CalendarEvents.ToList();
            DateTime dateTime = DateTime.Today;
            if (month != null)
                dateTime = new DateTime(dateTime.Year, Convert.ToInt32(month), 1);
            return View(dateTime);
        }

        public IActionResult Group(long groupId)
        {
            Schedule schedule = _db.Schedules.FirstOrDefault(g => g.GroupId == groupId);
            if (schedule != null)
                return View(schedule);

            return NotFound();
        }

        public IActionResult Create()
        {
            DateTime dateTime = DateTime.Today;
            ViewBag.Groups = _db.Groups.ToList();
            List<Schedule> schedules = _db.Schedules.ToList();
            long[] groupIdArray = new long[schedules.Count];
            for (int i = 0; i < schedules.Count; i++)
            {
                groupIdArray[i] = schedules[i].GroupId;
            }

            ViewBag.GroupIdArray = String.Join(" ", groupIdArray);
            return View(dateTime);
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
                    _db.Entry(calendarEvent).State = EntityState.Added;
                }
            }
            else
                return NotFound();
            
            await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(long? id)
        {
            Schedule schedule = _db.Schedules.FirstOrDefault(s => s.Id == id);
            if (schedule != null)
            {
                DateTime[] dateArray;

                /*dateArray = schedule.FromDate.OrderBy(d => d.Date).ToArray();
                string[] stringDates = new string[dateArray.Length];
                for (int i = 0; i < dateArray.Length; i++)
                {
                    var s = dateArray[i].Date.ToString("dd.MM.yyyy");
                    stringDates[i] = s;
                }

                ViewBag.DateArray = String.Join(" ", stringDates);

                TimeSpan time = dateArray[0].TimeOfDay;
                ViewBag.Time = time;*/
            }

            return View(schedule);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(long? groupId, TimeSpan scheduleTime, string multipleDates, string color)
        {
            int hour = scheduleTime.Hours;
            int minute = scheduleTime.Minutes;
            CultureInfo provider = CultureInfo.InvariantCulture;
            List<DateTime> datesFromString = new List<DateTime>();
            if (multipleDates != null)
            {
                List<string> datesString = multipleDates.Split("\r\n").ToList();
                datesFromString = datesString.Select(date => DateTime.ParseExact(date, "dd.MM.yyyy", provider))
                    .ToList();
                for (int i = 0; i < datesFromString.Count; i++)
                {
                    datesFromString[i] = new DateTime(datesFromString[i].Year, datesFromString[i].Month,
                        datesFromString[i].Day, hour, minute, 0);
                }
            }

            Schedule schedule = _db.Schedules.FirstOrDefault(s => s.GroupId == groupId);
 //           schedule.FromDate.Clear();
  //          schedule.ToDate.Clear();
 //           schedule.FromDate.AddRange(datesFromString);

            _db.Entry(schedule).State = EntityState.Modified;
            // добавить поле groupId в CalendarEvent!!! 
            foreach (var calendar in _db.CalendarEvents.Where(c => c.Group == schedule.Group.Name))
            {
                _db.Entry(calendar).State = EntityState.Deleted;
            }

            foreach (var date in datesFromString)
            {
                CalendarEvent calendarEvent = new CalendarEvent()
                {
              //      DayOfWeek = DayOfWeek.Friday,
                    Group = schedule.Group.Name,
                    Type = SelectBootstrapColor(color),
                    Action = $"/Schedule/Group/?groupId={schedule.GroupId}"
                };
                _db.Entry(calendarEvent).State = EntityState.Added;
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("Group", "Schedule", new {groupId = schedule.GroupId});
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



