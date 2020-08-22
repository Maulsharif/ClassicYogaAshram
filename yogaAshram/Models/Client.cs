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
        public string Sickness { get; set; }
        public string Source { get; set; }

        public long GroupId { get; set; }
        
        public virtual Group Group { get; set; }
        public long  CreatorId { get; set; }
        
        public virtual Employee Creator { get; set; }
        public List<string> Comments { get; set; }
        
        public Paid Paid { get; set; }
        public Contract Contract{ get; set; }
        public WhatsAppGroup WhatsAppGroup{ get; set; }
        public DateTime DateCreate { get; set; } = DateTime.Now;
        public long? MembershipId { get; set; }
        public virtual Membership Membership { get; set; }
        public virtual List<Attendance> Attendances { get; set; }

        public override string ToString()
        {
            return $"Я: {NameSurname}\n" +
                   "(Ф.И.О. полностью)\n" +
                   "настоящим подтверждаю, что с Правилами посещения и условиями абонемента йога- центра\n" +
                   "Classical Yoga Ashram ознакомлен и согласен. В дальнейшем иметь претензий не буду.\n" +
                   $"Информация о практикующем: {Source}\n" +
                   $"Место работы и должность: {WorkPlace}\n " +
                   $"Моб.: {PhoneNumber}\n" +
                   $"Дата рождения: {DateOfBirth}\n" +
                   $"E-mail: {Email}\n" +
                   $"Наличие заболеваний: {Sickness}\n" +
                   "Дата: \n" +
                   "Подпись: ";
        }
    }
    
}