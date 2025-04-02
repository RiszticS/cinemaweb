using AutoMapper;
using Cinema.DataAccess.Models;
using Cinema.Shared.Models;

namespace Cinema.WebAPI.Infrastructure;

/// <summary>
/// Automapper mappingProfile
/// </summary>
public class MappingProfile : Profile
{
    /// <summary>
    /// Constructor - Define mapping rules here
    /// </summary>
    public MappingProfile()
    {
        CreateMap<Movie, MovieResponseDto>(MemberList.Destination);

        CreateMap<Room, RoomResponseDto>(MemberList.Destination);

        CreateMap<Screening, ScreeningResponseDto>(MemberList.Destination);

        CreateMap<SeatRequestDto, Seat>(MemberList.Source)
            .ForSourceMember(s => s.Column, opt => opt.DoNotValidate())
            .ForSourceMember(s => s.Row, opt => opt.DoNotValidate())
            .ForMember(dest => dest.Position, opt => opt.MapFrom(src => new SeatPosition(src.Row, src.Column)));

        CreateMap<Seat, SeatResponseDto>(MemberList.Destination)
            .ForMember(dest => dest.Row, opt => opt.MapFrom(src => src.Position.Row))
            .ForMember(dest => dest.Column, opt => opt.MapFrom(src => src.Position.Column));

        CreateMap<ReservationRequestDto, Reservation>(MemberList.Source)
             .ForSourceMember(src => src.ScreeningId, opt => opt.DoNotValidate())
             .ForSourceMember(dest => dest.Seats, opt => opt.DoNotValidate())
             .ForMember(dest => dest.Seats, opt => opt.MapFrom<SeatResolver>());
        CreateMap<Reservation, ReservationResponseDto>(MemberList.Destination)
            .ForMember(dest => dest.Screening, opt => opt.MapFrom(src => src.Seats.First().Screening));
        CreateMap<SeatStatus, SeatStatusDto>(MemberList.Source);
    }
}

/// <summary>
/// SeatResolver
/// </summary>
public class SeatResolver : IValueResolver<ReservationRequestDto, Reservation, ICollection<Seat>>
{
    /// <summary>
    /// Resolve
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <param name="members"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public ICollection<Seat> Resolve(ReservationRequestDto source, Reservation destination, ICollection<Seat> members, ResolutionContext context)
    {
        return source.Seats.Select(s => new Seat
        {
            ScreeningId = (int)source.ScreeningId,
            Position = new SeatPosition(s.Row, s.Column),
            Status = SeatStatus.Reserved
        }).ToList();
    }
}
