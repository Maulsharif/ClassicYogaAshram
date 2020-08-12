
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

        public YogaAshramContext(DbContextOptions options) : base(options) 
        {
        }
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Membership>().HasData( 
        //         new {Id=1, Name = "12 посещении",  LessonNumbers= 12 },
        //         new { Id=2, Name = "8 посещении", LessonNumbers = 8 },
        //         new { Id=3, Name = "1 пробный",  LessonNumbers= 1 },
        //         new {  Id=4,Name = "3 пробных", LessonNumbers = 3 }
        //        
        //     );
        //     
        // }

        
    }
}