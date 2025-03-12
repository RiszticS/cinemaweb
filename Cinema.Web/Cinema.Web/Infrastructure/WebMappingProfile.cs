using AutoMapper;
using Cinema.DataAccess.Models;
using Cinema.Web.Models;

namespace Cinema.Web.Infrastructure
{
    public class WebMappingProfile : Profile
    {
        public WebMappingProfile()
        {
            CreateMap<Movie, MovieViewModel>(MemberList.Destination);
            CreateMap<Screening, ScreeningViewModel>(MemberList.Destination);
            CreateMap<Room, RoomViewModel>(MemberList.Destination);
        }
    }


}
