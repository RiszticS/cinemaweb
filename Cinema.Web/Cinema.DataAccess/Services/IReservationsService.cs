using Cinema.DataAccess.Models;

namespace Cinema.DataAccess.Services;

public interface IReservationsService
{
    Task AddAsync(long screeningId, Reservation reservation);
}