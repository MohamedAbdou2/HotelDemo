using AutoMapper;
using Presentation.ViewModels.User.Staff;

namespace Presentation.MappingProfile.Staff
{
    public class StaffProfile : Profile
    {
        public StaffProfile()
        {

            CreateMap<StaffRegisterViewModel, StaffRegisterViewModel>();



        }
    }
}
