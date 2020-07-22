using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace yogaAshram.Models
{
    public class YogaAshramContext : IdentityDbContext<Employee>
    {
        
        public YogaAshramContext(DbContextOptions options) : base(options)
        {
        }
    }
}