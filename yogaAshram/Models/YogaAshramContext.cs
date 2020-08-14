
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
        public YogaAshramContext(DbContextOptions options) : base(options) 
        {
        }
         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
             modelBuilder.Entity<ClientCategory>().HasData( 
                 new {Id=Convert.ToInt64(1), Name = "Пенсионеры"},
                 new {Id=Convert.ToInt64(2), Name = "Студенты"},
                 new {Id=Convert.ToInt64(3), Name = "Школьники"},
                 new {Id=Convert.ToInt64(4), Name = "Корпоратив"}
             );
             modelBuilder.Entity<Membership>().HasData( 
                 new {Id=Convert.ToInt64(1), Name = "12 разовый абонемент", AttendanceDays = 12, CategoryId = Convert.ToInt64(1)},
                 new {Id=Convert.ToInt64(2), Name = "8 разовый абонемент", AttendanceDays = 8, CategoryId = Convert.ToInt64(2)},
                 new {Id=Convert.ToInt64(3), Name = "8 разовый абонемент", AttendanceDays = 8, CategoryId = Convert.ToInt64(3)},
                 new {Id=Convert.ToInt64(4), Name = "12 разовый абонемент", AttendanceDays = 12, CategoryId = Convert.ToInt64(4)}
             );
         }

        
    }
}