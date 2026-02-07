using Application.Dtos.Room;
using AutoMapper;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles.Room
{
    public class RoomProfile : Profile
    {
        public RoomProfile()
        {
            CreateMap<Domain.Models.Room, Dtos.Room.GetRoomResponseDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type != null ? src.Type.Name : string.Empty))
                .ForMember(dest => dest.RoomPictures, opt => opt.MapFrom(src => src.RoomPictures != null 
                    ? src.RoomPictures.Where(rp => !rp.IsDeleted).Select(rp => rp.PictureUrl) 
                    : Enumerable.Empty<string>()));


            CreateMap<CreateRoomRequestDto, Domain.Models.Room>()
                .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => (Domain.Enums.RoomTypeCode)src.RoomTypeId))
                .ForMember(dest => dest.RoomPictures, opt =>
                opt.MapFrom(src => src.RoomPictures.Select(url =>
                    new RoomPicture
                    {
                        PictureUrl = url
                    })));

            CreateMap<UpdateRoomRequestDto,Domain.Models.Room>()
                .ForMember(dest => dest.RoomTypeId, opt => opt.MapFrom(src => (Domain.Enums.RoomTypeCode)src.RoomTypeId))
                .ForMember(dest => dest.RoomPictures, opt =>
                opt.MapFrom(src => src.RoomPictures.Select(url =>
                    new RoomPicture
                    {
                        PictureUrl = url
                    })));

        }
    }
}
