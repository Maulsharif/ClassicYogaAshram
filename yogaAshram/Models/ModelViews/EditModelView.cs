﻿using Microsoft.AspNetCore.Mvc;

 namespace yogaAshram.Models.ModelViews
{
    public class EditModelView
    {
        public string Id { get; set; }
        [Remote(action: "CheckUserNameCreate", controller: "Validation", ErrorMessage = "Такой логин уже существует")]
        public string UserName { get; set; }
        [Remote(action: "CheckEmailCreate", controller: "Validation", ErrorMessage = "Такая почта уже существует")]
        public string Email { get; set; }
        public string NameSurname { get; set; }
    }
}