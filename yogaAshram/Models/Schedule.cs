using System;
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
        [Required(ErrorMessage = "Введите значение")]
        
        public int FromHours { get; set; }
        [Required(ErrorMessage = "Введите значение")]
        public int FromMinutes { get; set; }
        public int ToHours { get; set; }
        [Required(ErrorMessage = "Введите значение")]
        public int ToMinutes { get; set; }
        public long GroupId { get; set; }
        public virtual Group Group{ get; set; }
        public virtual List<Group> Groups { get; set; }

        public string GetEnumValue()
        {
            string day = "";
            switch (DayOfWeek) 
            {
                case DayOfWeek.monday:
                    day = "понедельник";
                    break;
                case DayOfWeek.tuesday:
                    day = "вторник";
                    break;
                case DayOfWeek.wednesday:
                    day = "среду";
                    break;
                case DayOfWeek.thursday:
                    day = "четверг";
                    break;
                case DayOfWeek.friday:
                    day = "пятницу";
                    break;
                case DayOfWeek.saturday:
                    day = "субботу";
                    break;
                case DayOfWeek.sunday:
                    day = "воскресенье";
                    break;
            }

            return day;
        }
    }
   
}