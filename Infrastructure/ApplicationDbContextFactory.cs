using HotelDemo.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json
            // Look for appsettings.json in the Presentation project (startup project)
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Presentation");
            if (!Directory.Exists(basePath))
            {
                // Fallback to current directory if Presentation folder not found
                basePath = Directory.GetCurrentDirectory();
            }

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Create DbContext without IHttpContextAccessor for design-time operations
            // IHttpContextAccessor is not available during migrations/scaffolding
            return new ApplicationDbContext(optionsBuilder.Options, null!);
        }
    }
}

