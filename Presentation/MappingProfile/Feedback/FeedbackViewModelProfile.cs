
using Application.Dtos.Feedback;
using AutoMapper;
using Presentation.ViewModels.Feedback;

namespace Presentation.MappingProfile.Feedback
{

    public class FeedbackViewModelProfile : Profile
    {
        public FeedbackViewModelProfile()
        {
            // Create
            CreateMap<CreateFeedbackViewModel, CreateFeedbackDto>();

            // Update
            CreateMap<UpdateFeedbackViewModel, UpdateFeedbackDto>();

            // Response (if needed both ways)
            CreateMap<FeedbackDto, FeedbackViewModel>();
        }
    }

}
