using Cinema.DataAccess.Exceptions;
using Cinema.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Services;

internal class RoomsService : IRoomsService
{
    private readonly CinemaDbContext _context;

    public RoomsService(CinemaDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<Room>> GetAllAsync()
    {
        return await _context.Rooms
            .Where(r => !r.DeletedAt.HasValue)
            .ToListAsync();
    }

    public async Task<Room> GetByIdAsync(int id)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id && !r.DeletedAt.HasValue);
        if (room is null)
            throw new EntityNotFoundException(nameof(Room));

        return room;
    }
}