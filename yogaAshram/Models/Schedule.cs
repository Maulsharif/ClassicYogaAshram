using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


    namespace yogaAshram.Models
    {
        public class Schedule
        {
            public long Id { get; set; }
            public List<DateTime> FromDate { get; set; }
            public List<DateTime> ToDate { get; set; }
            public long GroupId { get; set; }
            public virtual Group Group{ get; set; }
            public virtual List<Group> Groups { get; set; }
        }
    }

   
