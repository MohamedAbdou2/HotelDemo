using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using HotelDemo.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Experimental;

namespace Infrastructure.DataSeeding
{
    public static class DataSeeder
    {
        public async static Task SeedData(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            if (!context.Users.Any())
            {
                var admin = new User
                {
                    Username = "admin",
                    FirstName = "admin",
                    LastName = "admin", 
                    Email = "Admin@Company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    DateOfBirth = new DateTime(1990, 1, 1)
                };

                context.Users.Add(admin);
                
                await context.SaveChangesAsync();

                var adminRoleId = context.Roles.FirstOrDefault(x => x.Name == "Admin")?.Id;
              
                if (adminRoleId.HasValue)
                {
                    var adminrole = new UserRole
                    {
                        UserId = admin.Id,  
                        RoleId = adminRoleId.Value,
                    };

                    context.UserRoles.Add(adminrole);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
