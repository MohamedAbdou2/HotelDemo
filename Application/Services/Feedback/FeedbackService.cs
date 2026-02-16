using System.Net.WebSockets;
using System.Numerics;
using Application.Dtos;
using Application.Dtos.Feedback;
using Application.Helper;
using Application.Interfaces;
using Application.Services.ReservationServices;
using Application.Validator.Feedback;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain.Enums;
using Domain.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;

namespace Application.Services.Feedback
{
    public class FeedbackService:IFeedbackService
    {
        private readonly IGenericRepository<Domain.Models.Feedback> feedbackrepo;
        private readonly CurrentUser currentUser;
        private readonly IValidator<CreateFeedbackDto> createDtoValidator;
        private readonly IValidator<UpdateFeedbackDto> udpateDtoValidator;
        private readonly RerservationService reservationServices;
        private readonly IMapper mapper;
        private readonly UserService userService;

        public FeedbackService(IGenericRepository<Domain.Models.Feedback> feedbackrepo 
            ,RerservationService reservationServices 
            ,IMapper mapper , UserService userService
            ,CurrentUser currentUser , IValidator<CreateFeedbackDto> createDtoValidator
            ,IValidator<UpdateFeedbackDto> udpateDtoValidator)
        {
            this.feedbackrepo = feedbackrepo;
            this.currentUser = currentUser;
            this.createDtoValidator = createDtoValidator;
            this.udpateDtoValidator = udpateDtoValidator;
            this.reservationServices = reservationServices;
            this.mapper = mapper;
            this.userService = userService;
        }

        public async Task<ResponseDto<bool>> CreateAsync(CreateFeedbackDto dto)
        {
            var validationResult = createDtoValidator.Validate(dto);
            if (!validationResult.IsValid)
                return ResponseDto<bool>.ValidationFail(validationResult);

            var feedbackcheck = await feedbackrepo.IsExist(x => x.ReservationId == dto.ReservationId);

            if (feedbackcheck)
                ResponseDto<bool>.Fail(ErrorCode.FeedbackAlreadyExist, "You already added feedback");

             dto.CustomerId = currentUser.GetUserId().Value;  

            var feedBack = mapper.Map<CreateFeedbackDto, Domain.Models.Feedback>(dto);

            var result =  await feedbackrepo.Add(feedBack);

            return result ? ResponseDto<bool>.Success(result, "Feedback added")
                : ResponseDto<bool>.Fail(ErrorCode.FailedtoAddFeedback, "Failed to Add Feedback");
        }


        public async Task<ResponseDto<FeedbackDto>> GetByIdAsync(Guid id)
        {
            var feedbackquerable = await feedbackrepo.GetbyId(id);
            var feedbackDto = await feedbackquerable.ProjectTo<FeedbackDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();

            if (feedbackDto == null)
                ResponseDto<FeedbackDto>.Fail(ErrorCode.FeedBackDoesNotExist, "Feed back not exist");

            return ResponseDto<FeedbackDto>.Success(feedbackDto);
        }


        public async Task<ResponseDto<bool>> UpdateAsync(Guid id, UpdateFeedbackDto dto)
        {
            var validationResult = udpateDtoValidator.Validate(dto);
            if (!validationResult.IsValid)
                return ResponseDto<bool>.ValidationFail(validationResult);

            var feedbackExist = await feedbackrepo.IsExist(x=>x.Id==id);

            if (!feedbackExist)
                ResponseDto<bool>.Fail(ErrorCode.FeedBackDoesNotExist, "Feed back not exist");

            var newfeedback = new Domain.Models.Feedback { Id = id};

            var modefiedparameters = typeof(UpdateFeedbackDto)
                .GetProperties()
                .Where(p => p.GetValue(dto) != null)
                .Select(p => p.Name)
                .ToArray(); ;

            var result = await feedbackrepo.UpdateIncludeAsync(newfeedback, modefiedparameters);

            return result ? ResponseDto<bool>.Success(result, "Feed back updated successfully")
                : ResponseDto<bool>.Fail(ErrorCode.FailedToUpdateFeedback, "Failed to Update feedback");
        }
        public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
        {
            var feedbackExist =await feedbackrepo.IsExist(x=>x.Id==id);

            if (!feedbackExist)
                ResponseDto<bool>.Fail(ErrorCode.FeedBackDoesNotExist, "Feed back not exist");

            var newfeedback = new Domain.Models.Feedback { Id = id};

            var result =await feedbackrepo.UpdateIncludeAsync(newfeedback, nameof(Domain.Models.Feedback.IsDeleted));

            return result ? ResponseDto<bool>.Success(result, "Feed back deleted")
                :ResponseDto<bool>.Fail(ErrorCode.FailedToDeleteFeedback , "Failed to delete feedback");
        }
    }
}
