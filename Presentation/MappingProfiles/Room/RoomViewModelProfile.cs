using Application.Dtos.Room;
using AutoMapper;
using Presentation.ViewModels.Room;

namespace Presentation.MappingProfiles.Room
{
    public class RoomViewModelProfile : Profile
    {
        public RoomViewModelProfile() {
            CreateMap<GetRoomResponseDto, GetRoomResponseViewModel>();
            CreateMap<CreateRoomRequestViewModel, CreateRoomRequestDto>();
            CreateMap<UpdateRoomRequestViewModel, UpdateRoomRequestDto>();
            CreateMap<RoomFilterRequestViewModel, RoomFilterRequestDto>();



        }
    }
}
