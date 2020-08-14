using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace yogaAshram.Models
{
    //Абонемент
    public class Membership
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int AttendanceDays { get; set; }
        public long CategoryId { get; set; }
        public virtual ClientCategory Category { get; set; }
    }
}