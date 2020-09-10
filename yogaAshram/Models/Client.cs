using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using yogaAshram.Controllers;
using yogaAshram.Models.ModelViews;

namespace yogaAshram.Models
{
    public enum ClientType
    {
        Probe,
        AreEngaged,
        NotEngaged
    }
    public enum Paid
    {
        Оплачено,
        Не_оплачено,
        Есть_долг
    }
    
    public enum WhatsAppGroup
    {
        Состоит_в_группе,
        Не_состоит_в_группе
    }
    public enum Contract
    {
        Есть_договор,
        Нет_договора
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
        public string Source { get; set; }

        public long GroupId { get; set; }
        
        public virtual Group Group { get; set; }
        public long  CreatorId { get; set; }
        
        public virtual Employee Creator { get; set; }
        public virtual List<Comment> Comments { get; set; }

        public int Balance { get; set; }
        public Paid Paid { get; set; }
        public Contract Contract{ get; set; }
        public WhatsAppGroup WhatsAppGroup{ get; set; }
        public DateTime DateCreate { get; set; } = DateTime.Now;
        public long? MembershipId { get; set; }
        
        public long? SicknessId { get; set; }
        public virtual Sickness Sickness { get; set; }
        public virtual Membership Membership { get; set; }
        
        [NotMapped]
        public virtual ClientsEditModelView ClientsEditModelView { get; set; }
        
    }
}