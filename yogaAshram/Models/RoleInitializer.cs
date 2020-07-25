using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace yogaAshram.Models
{
    public static class RoleInitializer
    {
        public static async Task Initialize(RoleManager<IdentityRole> roleManager,
            UserManager<Employee> userManager)
        {
            string dirEmail = "dir@gmail.com";
            string dirPassword = "12345678910";
            string dirNameSurname = "Checdasd";
            string dirUserName = "Chief";
            var roles = new[] { "chief", "manager", "seller", "marketer", "coach" };
            foreach (var role in roles)
            {
                if (await roleManager.FindByNameAsync(role) is null)
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
            if(await userManager.FindByEmailAsync(dirEmail) is null)
            {
                Employee chief = new Employee()
                {
                    Email = dirEmail,
                    UserName = dirUserName
                };
                var result = await userManager.CreateAsync(chief, dirPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(chief, "chief");
            }
        }
    }
}
