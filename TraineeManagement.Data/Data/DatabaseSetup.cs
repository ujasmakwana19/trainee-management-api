using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TraineeManagement.Data.DataBaseContext;

namespace TraineeManagement.Data.DatabaseSetup;

public static class DatabaseServiceExtensions
{
    // We had used the this cause is the extension method
    public static IServiceCollection AddDb(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection connection string is missing in configuration.");
        
        string versionString = configuration["MySql:ServerVersion"]
        ?? throw new InvalidOperationException("MySql:ServerVersion is missing in configuration.");

        Version version = Version.Parse(versionString);
        MySqlServerVersion serverVersion = new MySqlServerVersion(version);
        
        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(connectionString, serverVersion);
        });
        return services;
    }
}
