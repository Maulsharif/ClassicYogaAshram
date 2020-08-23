using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace yogaAshram.Models
{
    //Абонемент
    public class Membership
    {
        public long Id { get; set; }
        [Required(ErrorMessage = "Поле объязательно для заполнения")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Поле объязательно для заполнения")]
        public int Price { get; set; }
        [Required(ErrorMessage = "Поле объязательно для заполнения")]
        public int AttendanceDays { get; set; }
     
        public long CategoryId { get; set; }
        public virtual ClientCategory Category { get; set; }
      
    }
}