using Domain.Models;
using HotelDemo.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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
                // Seed Admin User
                var admin = new User
                {
                    Username = "admin",
                    FirstName = "admin",
                    PhoneNumber ="0123456789",
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

             
                var customerUser = new User
                {
                    Username = "customer",
                    FirstName = "John",
                    PhoneNumber = "0987654321",
                    LastName = "Doe",
                    Email = "customer@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Customer@123"),
                    DateOfBirth = new DateTime(1995, 5, 15)
                };

                context.Users.Add(customerUser);
                await context.SaveChangesAsync();

                var customer = new Customer
                {
                    UserId = customerUser.Id
                };
                context.Customers.Add(customer);

                var customerRoleId = context.Roles.FirstOrDefault(x => x.Name == "Customer")?.Id;

                if (customerRoleId.HasValue)
                {
                    var customerRole = new UserRole
                    {
                        UserId = customerUser.Id,
                        RoleId = customerRoleId.Value,
                    };

                    context.UserRoles.Add(customerRole);
                    await context.SaveChangesAsync();
                }

                // Seed Staff User
                var staffUser = new User
                {
                    Username = "staff",
                    FirstName = "Jane",
                    PhoneNumber = "0555123456",
                    LastName = "Smith",
                    Email = "staff@company.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Staff@123"),
                    DateOfBirth = new DateTime(1992, 8, 20)
                };

                context.Users.Add(staffUser);
                await context.SaveChangesAsync();
                var staff = new Staff
                {
                    UserId = staffUser.Id,
                    Position ="Staff"
                };
                context.Staff.Add(staff);
                var staffRoleId = context.Roles.FirstOrDefault(x => x.Name == "Staff")?.Id;

                if (staffRoleId.HasValue)
                {
                    var staffRole = new UserRole
                    {
                        UserId = staffUser.Id,
                        RoleId = staffRoleId.Value,
                    };

                    context.UserRoles.Add(staffRole);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
