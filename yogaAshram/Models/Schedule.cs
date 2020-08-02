using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace yogaAshram.Models
{
    public enum DayOfWeek
    {
        monday=1, tuesday, wednesday,thursday, friday, saturday,sunday
    }
    public class Schedule
    {
        public long Id { get; set; }
        public DayOfWeek DayOfWeek { get; set; } = DayOfWeek.monday;
        public int FromHours { get; set; }
        public int FromMinutes { get; set; }
        public int ToHours { get; set; }
        public int ToMinutes { get; set; }
        public int IdGroup { get; set; }
        public virtual Group Group{ get; set; }
        public virtual List<Group> Groups { get; set; }
    }
}