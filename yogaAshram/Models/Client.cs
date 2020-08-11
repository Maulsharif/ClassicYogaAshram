using System;
using yogaAshram.Controllers;

namespace yogaAshram.Models
{
    public enum ClientType
    {
        Probe,
        AreEngaged,
        NotEngaged
    }
   
    public class Client
    {
        public long Id { get; set; }
        
        public string NameSurname { get; set; }

        public string PhoneNumber { get; set; }

        public ClientType ClientType { get; set; }
        public int LessonNumbers  { get; set; }
        public string Color { get; set; }

        public long GroupId { get; set; }
        
        public virtual Group Group { get; set; }
        public long  CreatorId { get; set; }
        
        public virtual Employee Creator { get; set; }
        public string Comment{ get; set; }
        
    }
}