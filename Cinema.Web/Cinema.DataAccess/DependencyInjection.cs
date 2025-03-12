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
            .UseSqlServer(connectionString)
            .UseLazyLoadingProxies()
        );
        
        // Services
        //services.AddScoped<IMoviesService, MoviesSqlService>();
        services.AddScoped<IMoviesService, MoviesService>();
        services.AddScoped<IRoomsService, RoomsService>();
        services.AddScoped<IScreeningsService, ScreeningsService>();

        return services;
    }
}