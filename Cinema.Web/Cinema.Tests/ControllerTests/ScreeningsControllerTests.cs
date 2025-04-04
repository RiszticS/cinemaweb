using AutoMapper;
using Cinema.DataAccess;
using Cinema.DataAccess.Exceptions;
using Cinema.DataAccess.Models;
using Cinema.DataAccess.Services;
using Cinema.Shared.Models;
using Cinema.WebAPI.Controllers;
using Cinema.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cinema.Tests.ControllerTests;

public class ScreeningsControllerIntegrationTests: IDisposable
{
    private readonly CinemaDbContext _context;
    private readonly ScreeningsController _controller;

    public ScreeningsControllerIntegrationTests()
    {
        // Set up the in-memory database
        var options = new DbContextOptionsBuilder<CinemaDbContext>()
            .UseInMemoryDatabase("TestScreeningDatabase")
            .Options;
        _context = new CinemaDbContext(options);
        
        // Initialize dependencies
        var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()));
        var mapper = mapperConfig.CreateMapper();
        
        var screeningService = new ScreeningsService(_context);

        // Initialize the controller
        _controller = new ScreeningsController(mapper, screeningService);
        
        //Init database
        SeedDatabase();
    }

    #region Get

    [Fact]
    public async Task GetScreenings_ReturnsScreeningList()
    {
        // Arrange
        _context.Screenings.AddRange([
            new Screening { MovieId = 1, RoomId = 1, StartsAt = DateTime.UtcNow.AddHours(1) },
            new Screening { MovieId = 2, RoomId = 2, StartsAt = DateTime.UtcNow.AddHours(2) }
        ]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetScreenings(null, null, null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseDtos = Assert.IsType<List<ScreeningResponseDto>>(okResult.Value);
        Assert.Equal(2, responseDtos.Count);
    }
    
    [Fact]
    public async Task GetScreenings_ReturnsScreeningListForMovie()
    {
        // Arrange
        _context.Screenings.AddRange([
            new Screening { MovieId = 1, RoomId = 1, StartsAt = DateTime.UtcNow.AddHours(1) },
            new Screening { MovieId = 2, RoomId = 2, StartsAt = DateTime.UtcNow.AddHours(2) }
        ]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetScreenings(null, 2, null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseDtos = Assert.IsType<List<ScreeningResponseDto>>(okResult.Value);
        Assert.Single(responseDtos);
    }
    
    [Fact]
    public async Task GetScreenings_ReturnsScreeningListForRoom()
    {
        // Arrange
        _context.Screenings.AddRange([
            new Screening { MovieId = 1, RoomId = 1, StartsAt = DateTime.UtcNow.AddHours(1) },
            new Screening { MovieId = 2, RoomId = 2, StartsAt = DateTime.UtcNow.AddHours(2) }
        ]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetScreenings(null, 1, null, null);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseDtos = Assert.IsType<List<ScreeningResponseDto>>(okResult.Value);
        Assert.Single(responseDtos);
    }
    
    [Fact]
    public async Task GetScreenings_ReturnsScreeningListForInterval()
    {
        // Arrange
        _context.Screenings.AddRange([
            new Screening { MovieId = 1, RoomId = 1, StartsAt = DateTime.UtcNow.AddHours(1) },
            new Screening { MovieId = 2, RoomId = 2, StartsAt = DateTime.UtcNow.AddHours(5) }
        ]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetScreenings(null, null, DateTime.UtcNow.AddHours(1).AddMinutes(-1), DateTime.UtcNow.AddHours(4));

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseDtos = Assert.IsType<List<ScreeningResponseDto>>(okResult.Value);
        Assert.Single(responseDtos);
    }
    
    [Fact]
    public async Task GetScreenings_ReturnsScreeningWithAllParameters()
    {
        // Arrange
        _context.Screenings.AddRange([
            new Screening { MovieId = 1, RoomId = 1, StartsAt = DateTime.UtcNow.AddHours(1) },
            new Screening { MovieId = 2, RoomId = 2, StartsAt = DateTime.UtcNow.AddHours(5) }
        ]);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetScreenings(1, 1, DateTime.UtcNow.AddHours(1).AddMinutes(-1), DateTime.UtcNow.AddHours(5).AddMinutes(5));

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseDtos = Assert.IsType<List<ScreeningResponseDto>>(okResult.Value);
        Assert.Single(responseDtos);
    }

    [Fact]
    public async Task GetScreeningById_ThrowsNotFound_WhenDoesNotExist()
    {
        // Act && Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _controller.GetScreeningById(1));
    }
    
    [Fact]
    public async Task GetScreeningById_ReturnsScreening_WhenExists()
    {
        // Arrange
        var screening = new Screening
        {
            MovieId = 1,
            RoomId = 1,
            StartsAt = DateTime.UtcNow.AddHours(1)
        };

        _context.Screenings.Add(screening);
        await _context.SaveChangesAsync();

        // Act
        var result = await _controller.GetScreeningById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var responseDto = Assert.IsType<ScreeningResponseDto>(okResult.Value);
        Assert.Equal(screening.Id, responseDto.Id);
    }

    #endregion

    #region Helpers

    private void SeedDatabase()
    {
        if (_context.Movies.Any() || _context.Rooms.Any())
            return; // Prevent duplicate seeding

        // Seed Rooms
        var rooms = new List<Room>
        {
            new() { Name = "Room A", Rows = 10, Columns = 12, CreatedAt = DateTime.UtcNow },
            new() { Name = "Room B", Rows = 15, Columns = 15, CreatedAt = DateTime.UtcNow },
            new() { Name = "Room C", Rows = 20, Columns = 20, CreatedAt = DateTime.UtcNow },
        };

        // Seed Movies
        var movies = new List<Movie>
        {
            new()
            {
                Title = "Inception",
                Year = 2010,
                Director = "Christopher Nolan",
                Synopsis = "A skilled thief is offered a chance to have his criminal history erased as payment for the implantation of another person's idea into a target's subconscious.",
                Length = 148,
                Image = [], // Placeholder image data
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Title = "The Matrix",
                Year = 1999,
                Director = "The Wachowskis",
                Synopsis = "A computer hacker learns about the true nature of his reality and his role in the war against its controllers.",
                Length = 136,
                Image = [], // Placeholder image data
                CreatedAt = DateTime.UtcNow
            },
            new()
            {
                Title = "Interstellar",
                Year = 2014,
                Director = "Christopher Nolan",
                Synopsis = "A team of explorers travel through a wormhole in space in an attempt to ensure humanity's survival.",
                Length = 169,
                Image = [], // Placeholder image data
                CreatedAt = DateTime.UtcNow
            },
        };

        _context.Rooms.AddRange(rooms);
        _context.Movies.AddRange(movies);
        
        _context.SaveChanges();
        
        //Clear Change-tracker
        DetachAllEntities();
    }
    
    private void DetachAllEntities()
    {
        var trackedEntities = _context.ChangeTracker.Entries().ToList();
        foreach (var entry in trackedEntities)
        {
            entry.State = EntityState.Detached;
        }
    }

    #endregion
    
    public void Dispose()
    {
        _context.Database.EnsureDeleted(); // Deletes the in-memory database
        _context.Dispose();
    }
}