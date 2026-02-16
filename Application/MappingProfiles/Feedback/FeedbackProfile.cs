using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Feedback;
using AutoMapper;

namespace Application.MappingProfiles.Feedback
{
    public class FeedbackProfile:Profile
    {
        public FeedbackProfile()
        {
            CreateMap<CreateFeedbackDto, Domain.Models.Feedback>();


            CreateMap<UpdateFeedbackDto, Domain.Models.Feedback>();

            CreateMap<Domain.Models.Feedback, FeedbackDto>();
        }
    }
}
