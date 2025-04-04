using System.Net;
using System.Net.Http.Json;
using Cinema.DataAccess;
using Cinema.DataAccess.Models;
using Cinema.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.Tests.IntegrationTests;

public class MoviesControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public MoviesControllerIntegrationTests()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "IntegrationTest");
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace the real database with an in-memory database
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<CinemaDbContext>));
                if (descriptor != null) services.Remove(descriptor);

                services.AddDbContext<CinemaDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestMoviesDatabase");
                });

                //Seed the database with initial data
                using var scope = services.BuildServiceProvider().CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<CinemaDbContext>();
                db.Database.EnsureCreated();

                SeedMovies(db);
            });
        });

        _client = _factory.CreateClient();
    }

    #region Get

    [Fact]
    public async Task GetMovies_ReturnsAllMovies()
    {
        // Act
        var response = await _client.GetAsync("/movies");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var movies = await response.Content.ReadFromJsonAsync<List<MovieResponseDto>>();
        Assert.NotNull(movies);
        Assert.True(movies.Count >= 2); // Seeded with 2 movies
    }

    [Fact]
    public async Task GetMovieById_ReturnsMovie_WhenMovieExists()
    {
        // Arrange
        const int movieId = 1;

        // Act
        var response = await _client.GetAsync($"/movies/{movieId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var movie = await response.Content.ReadFromJsonAsync<MovieResponseDto>();
        Assert.NotNull(movie);
        Assert.Equal(movieId, movie.Id);
        Assert.Equal("Test Movie 1", movie.Title);
    }

    [Fact]
    public async Task GetMovieById_ReturnsNotFound_WhenMovieNotExists()
    {
        // Arrange
        var movieId = 99; // ID of a seeded movie

        // Act
        var response = await _client.GetAsync($"/movies/{movieId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.NotNull(problemDetails);
    }

    #endregion

    #region Helpers
    private void SeedMovies(CinemaDbContext context)
    {
        context.Movies.AddRange(
            new Movie { Title = "Test Movie 1", Year = 2020, Director = "Director 1", Synopsis = "Synopsis 1", Length = 120, Image = new byte[] { 1, 2, 3 } },
            new Movie { Title = "Test Movie 2", Year = 2021, Director = "Director 2", Synopsis = "Synopsis 2", Length = 90, Image = new byte[] { 1, 2, 3 } }
        );

        context.SaveChanges();
    }

    public void Dispose()
    {
        using var scope = _factory.Services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var db = scopedServices.GetRequiredService<CinemaDbContext>();
        db.Database.EnsureDeleted();

        _factory.Dispose();
        _client.Dispose();
    }

    #endregion
}