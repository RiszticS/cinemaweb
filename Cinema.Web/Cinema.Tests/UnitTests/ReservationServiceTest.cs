using Cinema.DataAccess;
using Cinema.DataAccess.Config;
using Cinema.DataAccess.Exceptions;
using Cinema.DataAccess.Models;
using Cinema.DataAccess.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;

namespace Cinema.Tests.UnitTests;

public class ReservationsServiceTests : IDisposable
{
    private readonly CinemaDbContext _context;
    private readonly ReservationsService _reservationsService;
    private readonly Mock<IUsersService> _mockUserService;

    public ReservationsServiceTests()
    {
        // Configure in-memory database
        var options = new DbContextOptionsBuilder<CinemaDbContext>()
            .UseInMemoryDatabase("TestReservationDatabase") // Unique database for each test
            .Options;

        _context = new CinemaDbContext(options);
        _mockUserService = new Mock<IUsersService>();

        // Configure ReservationSettings
        var reservationSettings = Options.Create(new ReservationSettings
        {
            MaximumNumberOfSeats = 6
        });

        // Mock email service
        Mock<IEmailsService> mockEmailService = new();

        // Initialize the ReservationsService
        _reservationsService = new ReservationsService(
            _context,
            reservationSettings,
            mockEmailService.Object,
            _mockUserService.Object);

        SeedDatabase();
    }

    #region Add

    [Fact]
    public async Task AddAsync_ThrowsNotFound_WhenScreeningNotExists()
    {
        var reservation = new Reservation
        {
            Email = "test@test.com",
            Name = "Test Reservation",
            Phone = "06301111111",
            Seats = new List<Seat>
            {
                new Seat { Position = new SeatPosition(2, 3) },
                new Seat { Position = new SeatPosition(2, 4) }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _reservationsService.AddAsync(99, reservation));
    }

    [Fact]
    public async Task AddAsync_ThrowsInvalidData_WhenDuplicatePositions()
    {
        var reservation = new Reservation
        {
            Email = "test@test.com",
            Name = "Test Reservation",
            Phone = "06301111111",
            Seats = new List<Seat>
            {
                new Seat { Position = new SeatPosition(2, 3) },
                new Seat { Position = new SeatPosition(2, 3) }
            }
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidDataException>(() => _reservationsService.AddAsync(1, reservation));
    }

    [Fact]
    public async Task AddAsync_ThrowsArgumentException_WhenAlreadyReserved()
    {
        var reservation = new Reservation
        {
            Email = "test@test.com",
            Name = "Test Reservation",
            Phone = "06301111111",
            Seats = new List<Seat>
            {
                new Seat { Position = new SeatPosition(2, 3) },
                new Seat { Position = new SeatPosition(2, 4) }
            }
        };

        _context.Reservations.Add(reservation);
        await _context.SaveChangesAsync();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _reservationsService.AddAsync(1, reservation));
    }

    [Fact]
    public async Task AddAsync_ThrowsArgumentException_WhenExceedingSeatLimit()
    {
        var reservation = new Reservation
        {
            Email = "test@test.com",
            Name = "Test Reservation",
            Phone = "06301111111",
            Seats = Enumerable.Range(1, 7).Select(i => new Seat
            {
                Position = new SeatPosition(i, 1)
            }).ToList()
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _reservationsService.AddAsync(1, reservation));
    }

    [Fact]
    public async Task AddAsync_AddsReservation()
    {
        var reservation = new Reservation
        {
            Email = "test@test.com",
            Name = "Test Reservation",
            Phone = "06301111111",
            Seats = new List<Seat>
            {
                new Seat { Position = new SeatPosition(2, 3), ScreeningId = 1},
                new Seat { Position = new SeatPosition(2, 4), ScreeningId = 1}
            }
        };

        // Act
        await _reservationsService.AddAsync(1, reservation);

        // Assert
        var reservations = await _context.Reservations.ToListAsync();
        Assert.Single(reservations);
    }

    #endregion


    #region Helper

    private void SeedDatabase()
    {
        var screening = new Screening
        {
            Movie = new Movie { Id = 1, Director = "Test Director", Length = 120, Year = 2024, Title = "Test Movie", CreatedAt = DateTime.UtcNow, Image = [], Synopsis = "" },
            Room = new Room { Id = 1, Rows = 10, Columns = 10, Name = "Room 1", CreatedAt = DateTime.UtcNow },
            Seats = new List<Seat>(),
            CreatedAt = DateTime.UtcNow,
            StartsAt = DateTime.Now.AddDays(1),
        };

        _context.Screenings.Add(screening);

        _context.SaveChanges();
    }

    #endregion

    public void Dispose()
    {
        _context.Database.EnsureDeleted(); // Deletes the in-memory database
        _context.Dispose();
    }
}