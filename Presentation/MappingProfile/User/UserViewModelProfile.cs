using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.User;
using AutoMapper;
using Presentation.ViewModels.User;

namespace Presentation.MappingProfiles.User
{
    public class UserViewModelProfile:Profile
    {
        public UserViewModelProfile()
        {
            CreateMap<RegisterViewModel, RegisterDto>();
             
        }
    }
}
