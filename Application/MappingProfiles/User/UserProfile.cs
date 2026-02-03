using Application.Dtos.User;
using AutoMapper;

namespace Application.MappingProfiles.User
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<RegisterDto, Domain.Models.User>()
                .ForMember(dst => dst.PasswordHash, o => o.MapFrom(r => BCrypt.Net.BCrypt.HashPassword(r.password)));
            //.ForMember(dst => dst.Username, o => o.MapFrom(r => BCrypt.Net.BCrypt.HashPassword(r.userName)));

            CreateMap<UpdateUserDto, Domain.Models.User>();
        }
    }
}
