
using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace yogaAshram.Models
{
    public class YogaAshramContext : IdentityDbContext<Employee, Role, long>
    {
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ManagerEmployee> ManagerEmployees { get; set; }
        public DbSet<Branch> Branches { get; set; }
        
        public DbSet<Group> Groups { get; set; }
     
        public DbSet<Schedule> Schedules { get; set; }
        
        public DbSet<Client> Clients { get; set; }
        public DbSet<CalendarEvent> CalendarEvents { get; set; }
        public DbSet<TrialUsers> TrialUserses { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<ClientCategory> Categories { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ClientsMembership> ClientsMemberships { get; set; }
        public YogaAshramContext(DbContextOptions options) : base(options) 
        {
        }
         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             modelBuilder.Entity<ClientCategory>().HasData( 
                 new {Id=Convert.ToInt64(1), Name = "Пенсионеры"}, //15% discount if not in group for pensioners, otherwise - 40%
                 new {Id=Convert.ToInt64(2), Name = "Студенты"}, //15% 
                 new {Id=Convert.ToInt64(3), Name = "Школьники"}, //15% discount if not in group for kids, otherwise - 20%
                 new {Id=Convert.ToInt64(4), Name = "Корпоратив"}, //Don't know yet. Let it be just 15%
                 new {Id=Convert.ToInt64(5), Name = "Без скидок"} 
             );
             modelBuilder.Entity<Membership>().HasData( 
                 new {Id=Convert.ToInt64(1), Name = "12 разовый абонемент в группу для пенсионеров (40% скидка)",
                     Price = 10000, AttendanceDays = 12, CategoryId = Convert.ToInt64(1)},
                 new {Id=Convert.ToInt64(2), Name = "8 разовый абонемент в группу для пенсионеров (40% скидка)",
                     Price = 8000, AttendanceDays = 8, CategoryId = Convert.ToInt64(1)},
                 
                 new {Id=Convert.ToInt64(3), Name = "12 разовый абонемент с 15% скидкой для пенсионеров",
                     Price = 14000, AttendanceDays = 12, CategoryId = Convert.ToInt64(1)},
                 new {Id=Convert.ToInt64(4), Name = "8 разовый абонемент с 15% скидкой для пенсионеров",
                     Price = 11000, AttendanceDays = 8, CategoryId = Convert.ToInt64(1)},
                 
                 new {Id=Convert.ToInt64(5), Name = "12 разовый абонемент с 15% скидкой для школьников",
                     Price = 14000, AttendanceDays = 12, CategoryId = Convert.ToInt64(3)},
                 new {Id=Convert.ToInt64(6), Name = "8 разовый абонемент с 15% скидкой для школьников",
                     Price = 11000, AttendanceDays = 8, CategoryId = Convert.ToInt64(3)},
                 
                 new {Id=Convert.ToInt64(7), Name = "12 разовый абонемент с 15% скидкой для студентов",
                     Price = 14000, AttendanceDays = 12, CategoryId = Convert.ToInt64(2)},
                 new {Id=Convert.ToInt64(8), Name = "8 разовый абонемент с 15% скидкой для студентов",
                     Price = 11000, AttendanceDays = 8, CategoryId = Convert.ToInt64(2)},
                 
                 new {Id=Convert.ToInt64(9), Name = "12 разовый абонемент с 15% скидкой для корпоративных клиентов",
                     Price = 14000, AttendanceDays = 12, CategoryId = Convert.ToInt64(4)},
                 new {Id=Convert.ToInt64(10), Name = "8 разовый абонемент с 15% скидкой для корпоративных клиентов",
                     Price = 11000, AttendanceDays = 8, CategoryId = Convert.ToInt64(4)},
                 
                 new {Id=Convert.ToInt64(11), Name = "12 разовый абонемент в детскую группу (20% скидка)",
                     Price = 13000, AttendanceDays = 12, CategoryId = Convert.ToInt64(3)},
                 new {Id=Convert.ToInt64(12), Name = "8 разовый абонемент в детскую группу (20% скидка)",
                     Price = 10500, AttendanceDays = 8, CategoryId = Convert.ToInt64(3)},
                 
                 new {Id=Convert.ToInt64(13), Name = "12 разовый абонемент обычный",
                     Price = 16000, AttendanceDays = 12, CategoryId = Convert.ToInt64(5)},
                 new {Id=Convert.ToInt64(14), Name = "8 разовый абонемент обычный",
                     Price = 13000, AttendanceDays = 8, CategoryId = Convert.ToInt64(5)},
                 
                 new {Id=Convert.ToInt64(15), Name = "4 разовый абонемент",
                     Price = 8000, AttendanceDays = 4, CategoryId = Convert.ToInt64(5)},
                 new {Id=Convert.ToInt64(16), Name = "1 разовый абонемент",
                     Price = 2500, AttendanceDays = 1, CategoryId = Convert.ToInt64(5)}
             );
         }

        
    }
}