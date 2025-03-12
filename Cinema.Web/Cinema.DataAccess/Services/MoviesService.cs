using Cinema.DataAccess.Exceptions;
using Cinema.DataAccess.Models;
using Cinema.DataAccess.Services;
using Cinema.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Services;

internal class MoviesService : IMoviesService
{
    private readonly CinemaDbContext _context;

    public MoviesService(CinemaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Movie>> GetLatestMoviesAsync(int? count = null)
    {
        var query = _context.Movies
            .Where(m => !m.DeletedAt.HasValue)
            .OrderByDescending(m => m.CreatedAt);

        if (count is null)
        {
            return await query.ToListAsync();
        }

        return await query
            .Take(count.Value)
            .ToListAsync();
    }

    public async Task<Movie> GetByIdAsync(int id)
    {
        var movie = await _context.Movies
            .FirstOrDefaultAsync(m => m.Id == id && !m.DeletedAt.HasValue);

        if (movie is null)
            throw new EntityNotFoundException(nameof(Movie));

        return movie;
    }
}