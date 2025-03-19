using Microsoft.EntityFrameworkCore;

namespace Cinema.DataAccess.Models;

[Owned]
public record SeatPosition(int Row, int Column);