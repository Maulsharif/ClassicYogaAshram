using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using yogaAshram.Controllers;

namespace yogaAshram.Models.ModelViews
{
    public class ClientsCreateModelView
    {
        [Required(ErrorMessage = "Введите Ф.И.О")]
        public string NameSurname { get; set; }
        
        [Required(ErrorMessage = "Введите номер телефона")]
        [Remote(action: "ClientPhoneNumber", controller: "Validation", ErrorMessage = "Ранее использовался этот номер")]
        public string PhoneNumber { get; set; }
        public long GroupId { get; set; }
        public virtual Group Group { get; set; }
        public ClientType ClientType { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int LessonNumbers  { get; set; }
      
        [Required(ErrorMessage = "Поле обязательно для заполнения")]

        [Remote(action: "CheckDate", controller: "Validation", ErrorMessage = "Некоректная дата")]
        public DateTime StartDate { get; set; }
        public string Comment { get; set; }
        
        public long SicknessId { get; set; }
        public Sickness Sickness { get; set; }
        
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string Source { get; set; }
    
      
        [RegularExpression (@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}", ErrorMessage = "Некорректный email")]
        public string Email { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string WorkPlace { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public DateTime DateOfBirth { get; set; }

        public long MembershipId { get; set; }
        
      
        
    }
}