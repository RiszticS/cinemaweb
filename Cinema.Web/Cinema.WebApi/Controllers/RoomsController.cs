using AutoMapper;
using Cinema.DataAccess.Services;
using Cinema.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.WebAPI.Controllers;

/// <summary>
/// RoomsController
/// </summary>
[ApiController]
[Route("/rooms")]
public class RoomsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IRoomsService _roomsService;

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="mapper"></param>
    /// <param name="roomsService"></param>
    public RoomsController(IMapper mapper, IRoomsService roomsService)
    {
        _mapper = mapper;
        _roomsService = roomsService;
    }
    
    /// <summary>
    /// Get all rooms
    /// </summary>
    /// <response code="200">A list of rooms</response>
    [HttpGet]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(List<RoomResponseDto>))]
    public async Task<IActionResult> GetRooms()
    {
        var rooms = await _roomsService.GetAllAsync();
        var roomResponseDtos = _mapper.Map<List<RoomResponseDto>>(rooms);

        return Ok(roomResponseDtos);
    }

    /// <summary>
    /// Get a room by ID
    /// </summary>
    /// <param name="id"></param>
    /// <response code="200">The requested room</response>
    /// <response code="404">Not found</response>
    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(statusCode: StatusCodes.Status200OK, type: typeof(RoomResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetRoomById([FromRoute] int id)
    {
        var room = await _roomsService.GetByIdAsync(id);
        var roomResponseDto = _mapper.Map<RoomResponseDto>(room);

        return Ok(roomResponseDto);
    }
}
