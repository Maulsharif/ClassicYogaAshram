﻿using System.ComponentModel.DataAnnotations;
 using Microsoft.AspNetCore.Mvc;

 namespace yogaAshram.Models.ModelViews
{
    public class ManagerEditModelView
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Введите логин")]
        [Remote(action: "CheckUserNameForEditing", controller: "Validation", ErrorMessage = "Аккаунт с таким логином уже зарегистрирован", AdditionalFields = nameof(Id))]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Введите почту")]
        [Remote(action: "CheckEmailForEditing", controller: "Validation", ErrorMessage = "Аккаунт с такой почтой уже зарегистрирован", AdditionalFields = nameof(Id))]
        public string Email { get; set; }
        [Required(ErrorMessage = "Введите имя и фамилию")]
        public string NameSurname { get; set; }
    }
}