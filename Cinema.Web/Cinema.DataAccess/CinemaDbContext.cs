using Cinema.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess
{
    class CinemaDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; } = null!;

        public CinemaDbContext(DbContextOptions<CinemaDbContext> options)
            : base(options)
        {
        }
    }
}
