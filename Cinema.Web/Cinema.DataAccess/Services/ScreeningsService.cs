using Cinema.DataAccess.Exceptions;
using Cinema.DataAccess.Models;
using Cinema.DataAccess.Services;
using Cinema.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Services;

public class ScreeningsService : IScreeningsService
{
    private readonly CinemaDbContext _context;

    public ScreeningsService(CinemaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Screening>> GetForDateAsync(DateTime date)
    {
        return await _context.Screenings
             .Where(s => s.StartsAt.Date == date.Date)
             .OrderBy(s => s.StartsAt)
             .ToListAsync();
    }

    public async Task<Screening> GetByIdAsync(int id)
    {
        var screening = await _context.Screenings.FirstOrDefaultAsync(s => s.Id == id);
        if (screening is null)
            throw new EntityNotFoundException(nameof(screening));

        return screening;
    }

    public async Task<IReadOnlyCollection<Screening>> GetAllAsync(int? movieId, int? roomId, DateTime? from = null, DateTime? until = null)
    {
        var query = _context.Screenings.AsQueryable();
        if (from.HasValue)
        {
            query = query.Where(s => s.StartsAt >= from);
        }

        if (until.HasValue)
        {
            query = query.Where(s => s.StartsAt <= until);
        }

        query = query
            .Where(s =>
                (!movieId.HasValue || s.MovieId == movieId)
                && (!roomId.HasValue || s.RoomId == roomId))
            .OrderBy(s => s.StartsAt);

        return await query.ToListAsync();
    }

    public async Task<List<Seat>> GetSeatsByScreeningAsync(int id)
    {
        var seats = await _context.Seats
            .Where(s => s.ScreeningId == id)
            .ToListAsync();

        return seats;
    }
}