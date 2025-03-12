using Cinema.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
    {
        // Database
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<CinemaDbContext>(options => options
            .UseMySQL(connectionString)
        );

        // Services
        services.AddScoped<IMoviesService, MoviesSqlService>();


        return services;
    }
}