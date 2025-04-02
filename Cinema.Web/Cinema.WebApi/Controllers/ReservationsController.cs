using AutoMapper;
using Cinema.DataAccess.Models;
using Cinema.DataAccess.Services;
using Cinema.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.WebAPI.Controllers;

/// <summary>
/// ReservationsController
/// </summary>
[ApiController]
[Route("/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IReservationsService _reservationsService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="reservationsService"></param>
    public ReservationsController(IMapper mapper, IReservationsService reservationsService)
    {
        _mapper = mapper;
        _reservationsService = reservationsService;
    }

    /// <summary>
    /// Add a new reservation
    /// </summary>
    /// <param name="reservationRequestDto"></param>
    /// <response code="201">Reservation created successfully</response>
    /// <response code="400">Bad Request</response>
    /// <response code="409">Conflict</response>
    [HttpPost]
    [ProducesResponseType(statusCode: StatusCodes.Status201Created, type: typeof(ReservationResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateReservation([FromBody] ReservationRequestDto reservationRequestDto)
    {
        var reservation = _mapper.Map<Reservation>(reservationRequestDto);
        await _reservationsService.AddAsync(reservationRequestDto.ScreeningId, reservation);

        var reservationResponseDto = _mapper.Map<ReservationResponseDto>(reservation);
        
        return CreatedAtAction(nameof(CreateReservation), new { id = reservationResponseDto.Id }, reservationResponseDto);
    }
    
}
