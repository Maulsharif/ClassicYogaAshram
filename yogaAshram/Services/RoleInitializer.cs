using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using yogaAshram.Models;

namespace yogaAshram.Services
{
    public static class RoleInitializer
    {
        public static async Task Initialize(RoleManager<IdentityRole> roleManager,
            UserManager<Employee> userManager)
        {
            string managerEmail = "manager@gmail.com";
            string managerPassword = "12345678910";
            string managerNameSurname = "ManagerS";
            string managerUserName = "Manager";
            
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
            if (await userManager.FindByEmailAsync(dirEmail) is null)
            {
                Employee chief = new Employee()
                {
                    Email = dirEmail,
                    UserName = dirUserName,
                    NameSurname = dirNameSurname
                };
                var result = await userManager.CreateAsync(chief, dirPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(chief, "chief");
            }

            if (await userManager.FindByEmailAsync(managerEmail) is null)
            {
                Employee manager = new Employee()
                {
                    Email = managerEmail,
                    UserName = managerUserName,
                    NameSurname = managerNameSurname
                };
                var result = await userManager.CreateAsync(manager, managerPassword);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(manager, "manager");
            }
        }
    }
}
