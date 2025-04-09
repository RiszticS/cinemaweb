using Cinema.DataAccess.Exceptions;
using Cinema.DataAccess.Models;
using Cinema.DataAccess.Services;
using Cinema.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Services;

public class MoviesService : IMoviesService
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

    public async Task AddAsync(Movie movie)
    {
        await CheckIfTitleExistsAsync(movie);

        movie.CreatedAt = DateTime.UtcNow;

        try
        {
            await _context.Movies.AddAsync(movie);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new SaveFailedException("Failed to create movie.", ex);
        }
    }

    public async Task UpdateAsync(Movie movie)
    {
        var exisingMovie = await GetByIdAsync(movie.Id);

        if (exisingMovie == null)
            throw new EntityNotFoundException(nameof(Movie));

        if (exisingMovie.Screenings.Count != 0 && movie.Length > exisingMovie.Length)
            throw new InvalidDataException("Movie cannot be longer because screenings already exists");

        await CheckIfTitleExistsAsync(movie);
        movie.CreatedAt = exisingMovie.CreatedAt;

        try
        {
            _context.Entry(exisingMovie).State = EntityState.Detached;
            _context.Movies.Update(movie);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new SaveFailedException("Failed to update movie.", ex);
        }
    }

    public async Task DeleteAsync(int id)
    {
        var movie = await GetByIdAsync(id);

        movie.DeletedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            throw new SaveFailedException("Failed to delete movie.", ex);
        }
    }

    private async Task CheckIfTitleExistsAsync(Movie movie)
    {
        if (await _context.Movies.AnyAsync(m => m.Id != movie.Id
                                                && !m.DeletedAt.HasValue
                                                && m.Title.ToLower().Equals(movie.Title.ToLower())))
            throw new InvalidDataException("Movie with the same name already exists");
    }
}