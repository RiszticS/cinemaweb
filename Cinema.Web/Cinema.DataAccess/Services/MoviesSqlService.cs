using Cinema.DataAccess.Exceptions;
using Cinema.DataAccess.Models;
using Cinema.DataAccess.Services;
using Cinema.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Services;

// FromSql is safe against sql injections
// https://learn.microsoft.com/en-us/ef/core/querying/sql-queries?tabs=sqlserver#passing-parameters

internal class MoviesSqlService : IMoviesService
{
    private readonly CinemaDbContext _context;

    public MoviesSqlService(CinemaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Movie>> GetLatestMoviesAsync(int? count = null)
    {
        if (count is null)
        {
            return await _context.Movies.FromSql(
                $"""
                 SELECT
                 Id, Title, Year, Director, Synopsis, Length, Image, CreatedAt, DeletedAt
                 FROM Movies
                 WHERE DeletedAt is null
                 ORDER BY CreatedAt DESC
                 """
            ).ToListAsync();
        }

        return await _context.Movies.FromSql(
            $"""
             SELECT TOP ({count.Value})
             Id, Title, Year, Director, Synopsis, Length, Image, CreatedAt, DeletedAt
             FROM Movies
             WHERE DeletedAt is null
             ORDER BY CreatedAt DESC
             """
        ).ToListAsync();
    }

    public async Task<Movie> GetByIdAsync(int id)
    {
        var movie = await _context.Movies
            .FromSql(
                $"""
                   SELECT *    
                   FROM movies
                   WHERE Id = {id}
                   AND DeletedAt is null
                 """)
            .FirstOrDefaultAsync();

        if (movie is null)
            throw new EntityNotFoundException(nameof(Movie));

        return movie;
    }

}