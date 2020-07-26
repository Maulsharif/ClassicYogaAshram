
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace yogaAshram.Models
{
    public class YogaAshramContext : IdentityDbContext<Employee>
    {
        public DbSet<Employee> Employees { get; set; }

        public YogaAshramContext(DbContextOptions options) : base(options) 
        {
        }
    }
}