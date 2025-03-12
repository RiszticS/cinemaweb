using Cinema.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess;

public class CinemaDbContext : DbContext
{
    public DbSet<Movie> Movies { get; set; } = null!;
    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Screening> Screenings { get; set; } = null!;

    public CinemaDbContext(DbContextOptions<CinemaDbContext> options)
        : base(options)
    {
    }
}