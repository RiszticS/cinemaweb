using Cinema.DataAccess.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.DataAccess
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration config)
        {
            // Services
            services.AddScoped<IMoviesService, MoviesSqlService>();

            return services;
        }
    }
}
