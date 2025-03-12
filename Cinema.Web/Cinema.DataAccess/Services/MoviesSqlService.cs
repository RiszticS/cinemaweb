using Cinema.DataAccess;
using Cinema.DataAccess.Exceptions;
using Cinema.DataAccess.Models;
using Microsoft.EntityFrameworkCore;


namespace Cinema.DataAccess.Services
{

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
                SELECT
                Id, Title, Year, Director, Synopsis, Length, Image, CreatedAt, DeletedAt
                FROM Movies
                WHERE DeletedAt is null
                ORDER BY CreatedAt DESC
                LIMIT {count.Value}
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
}