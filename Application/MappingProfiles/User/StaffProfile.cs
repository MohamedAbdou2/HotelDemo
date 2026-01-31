using Application.Dtos.User;
using Application.Dtos.User.Staff;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.MappingProfiles.User
{
    public class StaffProfile : Profile

    {
        public StaffProfile()
        {
            CreateMap<StaffRegisterDto, Domain.Models.User>()
               .ForMember(dst => dst.PasswordHash, o => o.MapFrom(r => BCrypt.Net.BCrypt.HashPassword(r.password)));
               //.ForMember(dst => dst.Username, o => o.MapFrom(r => BCrypt.Net.BCrypt.HashPassword(r.userName)));

        }



    }
}
