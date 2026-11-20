using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace RaceDay.API.Data
{
    // Design-time factory used by Entity Framework Core tools for migrations and scaffolding
    public class RaceDayDbContextFactory : IDesignTimeDbContextFactory<RaceDayDbContext>
    {
        // Creates a DbContext instance when executing EF Core CLI or Package Manager commands
        public RaceDayDbContext CreateDbContext(string[] args)
        {
            // Build configuration to read database settings from appsettings.json
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            // Configure DbContext options using the connection string from configuration
            var builder = new DbContextOptionsBuilder<RaceDayDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            // Return configured database context instance
            return new RaceDayDbContext(builder.Options);
        }
    }
}