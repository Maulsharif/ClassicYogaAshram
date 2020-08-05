
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

        public YogaAshramContext(DbContextOptions options) : base(options) 
        {
        }
    }
}