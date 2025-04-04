using Cinema.DataAccess.Models;

namespace Cinema.DataAccess.Services;

public interface IReservationsService
{
    Task<List<Reservation>> GetAllReservationsAsync();
    Task<Reservation> GetByIdAsync(int id);
    Task AddAsync(long screeningId, Reservation reservation);
    Task CancelAsync(int id);
}