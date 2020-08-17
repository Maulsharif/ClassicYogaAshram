using System;
using System.Collections.Generic;
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
        public DateTime DateOfBirth { get; set; }

        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string WorkPlace { get; set; }
        public ClientType ClientType { get; set; }
        public int LessonNumbers  { get; set; }
        
        public string Color { get; set; }
        public string Sickness { get; set; }
        public string Source { get; set; }

        public long GroupId { get; set; }
        
        public virtual Group Group { get; set; }
        public long  CreatorId { get; set; }
        
        public virtual Employee Creator { get; set; }
        public string Comment{ get; set; }
        
        public bool Paid { get; set; }
        public bool Contract { get; set; }
        public bool WhatsAppGroup { get; set; }
        
        public long? MembershipId { get; set; }
        public virtual Membership Membership { get; set; }
        public virtual List<Membership> Memberships { get; set; }
      
    }
}