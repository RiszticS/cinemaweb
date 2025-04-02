using AutoMapper;
using Cinema.DataAccess.Services;
using Cinema.DataAccess.Exceptions;
using Cinema.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.WebAPI.Controllers;

/// <summary>
/// MoviesController
/// </summary>
[ApiController]
[Route("/movies")]
public class MoviesController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMoviesService _moviesService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="moviesService"></param>
    public MoviesController(IMapper mapper,
        IMoviesService moviesService)
    {
        _mapper = mapper;
        _moviesService = moviesService;
    }

    /// <summary>
    /// Get all movies in descending order by creation date
    /// </summary>
    /// <param name="count">An optional parameter to restrict the number of the returned movies</param>
    /// <response code="200">A list of movies</response>
    [HttpGet]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<MovieResponseDto>))]
    public async Task<IActionResult> GetMovies(int? count = null)
    {
        var movies = await _moviesService.GetLatestMoviesAsync(count);
        var movieResponseDtos = _mapper.Map<List<MovieResponseDto>>(movies);

        return Ok(movieResponseDtos);
    }

    /// <summary>
    /// Get a movie by ID
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">The requested movie</response>
    /// <response code="404">Not found</response>
    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(MovieResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMovieById([FromRoute] int id)
    {
        var movie = await _moviesService.GetByIdAsync(id);
        var movieResponseDto = _mapper.Map<MovieResponseDto>(movie);

        return Ok(movieResponseDto);
    }

}
