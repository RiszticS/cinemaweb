using Cinema.DataAccess.Config;
using Cinema.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
    {
        // Config
        services.Configure<ReservationSettings>(config.GetSection("ReservationSettings"));
        services.Configure<EmailSettings>(config.GetSection("EmailSettings"));

        // Database
        var connectionString = config.GetConnectionString("DefaultConnection");
        services.AddDbContext<CinemaDbContext>(options => options
            .UseSqlServer(connectionString)
            .UseLazyLoadingProxies()
        );

        // Services
        services.AddScoped<IMoviesService, MoviesService>();
        // services.AddScoped<IMoviesService, MoviesSqlService>();
        services.AddScoped<IRoomsService, RoomsService>();
        services.AddScoped<IScreeningsService, ScreeningsService>();
        services.AddScoped<IReservationsService, ReservationsService>();

        // Add email sending service
        services.AddSingleton<IEmailsService, SmtpEmailsService>();

        return services;
    }
}