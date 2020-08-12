using System;

namespace yogaAshram.Models
{
    public enum State
    {
        willAttend=1,
        attended,
        notattended
    }
   
    public class TrialUsers
    {
        public long Id { get; set; }
        public long ClientId { get; set; }
        public virtual  Client Client { get; set; }
        public long GroupId { get; set; }
        public virtual  Group Group { get; set; }
        public DateTime LessonTime { get; set; }
        public State State { get; set; } = State.willAttend;
        public string Color { get; set; } = "grey";
        public bool IsChecked { get; set; } = false;

        public string GetValueOfState()
        {
            string res = "";
            switch (State)
            {
                case State.willAttend:
                    res= "придет";
                    break;
      
                case State.notattended:
                    res= "не пришел";
                    break;
                case State.attended:
                    res= "пришел";
                    break;
            }

            return res;
        }

    }
}