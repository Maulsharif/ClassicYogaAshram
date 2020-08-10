using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


    namespace yogaAshram.Models
    {
        public class Schedule
        {
            public long Id { get; set; }
            public long BranchId { get; set; }
            public virtual Branch Branch { get; set; }
            public List<string> DayOfWeeksString { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan FinishTime { get; set; }
            public long GroupId { get; set; }
            public virtual Group Group{ get; set; }
            public virtual List<Group> Groups { get; set; }
        }
    }

   
