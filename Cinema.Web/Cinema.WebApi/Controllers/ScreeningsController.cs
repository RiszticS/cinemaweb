using AutoMapper;
using Cinema.DataAccess.Services;
using Cinema.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.WebAPI.Controllers;

/// <summary>
/// ScreeningsController
/// </summary>
[ApiController]
[Route("/screenings")]
public class ScreeningsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IScreeningsService _screeningsService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="screeningsService"></param>
    public ScreeningsController(IMapper mapper, IScreeningsService screeningsService)
    {
        _mapper = mapper;
        _screeningsService = screeningsService;
    }
    
    /// <summary>
    /// Get all screenings
    /// </summary>
    /// <param name="movieId"></param>
    /// <param name="roomId"></param>
    /// <param name="startsAfter"></param>
    /// <param name="startsBefore"></param>
    /// <response code="200">A list of screenings</response>
    /// <response code="400">Bad Request</response>
    [HttpGet]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<ScreeningResponseDto>))]
    public async Task<IActionResult> GetScreenings([FromQuery] int? movieId, [FromQuery] int? roomId, [FromQuery] DateTime? startsAfter, [FromQuery] DateTime? startsBefore)
    {
        var screenings = await _screeningsService.GetAllAsync(movieId, roomId, startsAfter, startsBefore);
        var screeningResponseDtos = _mapper.Map<List<ScreeningResponseDto>>(screenings);

        return Ok(screeningResponseDtos);
    }

    /// <summary>
    /// Get a screening by ID
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">The requested screening</response>
    /// <response code="404">Not found</response>
    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(ScreeningResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetScreeningById([FromRoute] int id)
    {
        var screening = await _screeningsService.GetByIdAsync(id);
        var screeningResponseDto = _mapper.Map<ScreeningResponseDto>(screening);

        return Ok(screeningResponseDto);

    }

    /// <summary>
    /// Get all reserved/sold seats for the given screening
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">A list of seats</response>
    /// <response code="404">Not found</response>
    [HttpGet]
    [Route("{id}/seats")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<SeatResponseDto>))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSeatsByScreening([FromRoute] int id)
    {
        var seats = await _screeningsService.GetSeatsByScreeningAsync(id);
        var seatResponseDtos = _mapper.Map<List<SeatResponseDto>>(seats);

        return Ok(seatResponseDtos);
    }
}
