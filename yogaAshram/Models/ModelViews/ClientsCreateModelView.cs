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
        [Remote(action: "ClientPhoneNumber", controller: "Validation", ErrorMessage = "Ранее использовался этот номер !!!")]
        public string PhoneNumber { get; set; }
        public long GroupId { get; set; }
        public virtual Group Group { get; set; }
        public ClientType ClientType { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public int LessonNumbers  { get; set; }
        public string Color { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]

        [Remote(action: "CheckDate", controller: "Validation", ErrorMessage = "Некоректная дата")]
        public DateTime StartDate { get; set; }
        public string Comment { get; set; }
        
    }
}