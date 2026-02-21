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
using Domain.Models;
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
        private readonly IReservationService reservationServices;
        private readonly IGenericRepository<Customer> customerRepo;
        private readonly IMapper mapper;

        public FeedbackService(IGenericRepository<Domain.Models.Feedback> feedbackrepo 
            ,IReservationService reservationServices 
            ,IGenericRepository<Customer> customerRepo
            ,IMapper mapper
            ,CurrentUser currentUser , IValidator<CreateFeedbackDto> createDtoValidator
            ,IValidator<UpdateFeedbackDto> udpateDtoValidator)
        {
            this.feedbackrepo = feedbackrepo;
            this.currentUser = currentUser;
            this.createDtoValidator = createDtoValidator;
            this.udpateDtoValidator = udpateDtoValidator;
            this.reservationServices = reservationServices;
            this.customerRepo = customerRepo;
            this.mapper = mapper;
        }

        public async Task<ResponseDto<bool>> CreateAsync(CreateFeedbackDto dto)
        {
            var validationResult = createDtoValidator.Validate(dto);
            if (!validationResult.IsValid)
                return ResponseDto<bool>.ValidationFail(validationResult);

            var createFeedbackValidationResponse =await CreateFeedbackValidator(dto);

            if (!createFeedbackValidationResponse.IsSuccess)
                return ResponseDto<bool>.Fail(createFeedbackValidationResponse.ErrorCode , createFeedbackValidationResponse.Message);

            var feedBack = mapper.Map<CreateFeedbackDto, Domain.Models.Feedback>(dto);

            var result =  await feedbackrepo.Add(feedBack);

            return result ? ResponseDto<bool>.Success(result, "Feedback added")
                : ResponseDto<bool>.Fail(ErrorCode.FailedtoAddFeedback, "Failed to Add Feedback");
        }

        private async Task<ResponseDto<CreateFeedbackDto>> CreateFeedbackValidator(CreateFeedbackDto dto)
        {
            var feedbackcheck = await feedbackrepo.IsExist(x => x.ReservationId == dto.ReservationId);

            if (feedbackcheck)
               return ResponseDto<CreateFeedbackDto>.Fail(ErrorCode.FeedbackAlreadyExist, "You already added feedback");

            if (!await reservationServices.IsReservationExist(dto.ReservationId))
               return ResponseDto<CreateFeedbackDto>.Fail(ErrorCode.ReservationNotFound, "No reservation with this Id");


            var userId = currentUser.GetUserId().Value;
            var customerId = await customerRepo.GetAll(x => x.UserId == userId).Select(x => x.Id).FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(customerId.ToString()))
               return ResponseDto<CreateFeedbackDto>.Fail(ErrorCode.CustomerNotFound, "No Customer with this id");

            dto.CustomerId = customerId;

            return ResponseDto<CreateFeedbackDto>.Success(dto);
        }

        public async Task<ResponseDto<FeedbackDto>> GetByIdAsync(Guid reservationId)
        {
            var feedbackDto = await feedbackrepo.GetAll(x=>x.ReservationId==reservationId).ProjectTo<FeedbackDto>(mapper.ConfigurationProvider).FirstOrDefaultAsync();

            if (feedbackDto == null)
                return ResponseDto<FeedbackDto>.Fail(ErrorCode.FeedBackDoesNotExist, "Feed back not exist");

            return ResponseDto<FeedbackDto>.Success(feedbackDto);
        }


        public async Task<ResponseDto<bool>> UpdateAsync(Guid id, UpdateFeedbackDto dto)
        {
            var validationResult = udpateDtoValidator.Validate(dto);
            if (!validationResult.IsValid)
                return ResponseDto<bool>.ValidationFail(validationResult);

            var feedbackExist = await feedbackrepo.IsExist(x=>x.Id==id);

            if (!feedbackExist)
               return ResponseDto<bool>.Fail(ErrorCode.FeedBackDoesNotExist, "Feed back not exist");

            var newfeedback = new Domain.Models.Feedback { Id = id};

            mapper.Map(dto, newfeedback);

            var modefiedparameters = typeof(UpdateFeedbackDto)
                .GetProperties()
                .Where(p => p.GetValue(dto) != null)
                .Select(p => p.Name)
                .ToArray();
            var result = await feedbackrepo.UpdateIncludeAsync(newfeedback, modefiedparameters);

            return result ? ResponseDto<bool>.Success(result, "Feed back updated successfully")
                : ResponseDto<bool>.Fail(ErrorCode.FailedToUpdateFeedback, "Failed to Update feedback");
        }
        public async Task<ResponseDto<bool>> DeleteAsync(Guid id)
        {
            var feedbackExist =await feedbackrepo.IsExist(x=>x.Id==id);

            if (!feedbackExist)
               return ResponseDto<bool>.Fail(ErrorCode.FeedBackDoesNotExist, "Feed back not exist");

            var newfeedback = new Domain.Models.Feedback { Id = id , IsDeleted=true};

            var result =await feedbackrepo.UpdateIncludeAsync(newfeedback, nameof(Domain.Models.Feedback.IsDeleted));

            return result ? ResponseDto<bool>.Success(result, "Feed back deleted")
                :ResponseDto<bool>.Fail(ErrorCode.FailedToDeleteFeedback , "Failed to delete feedback");
        }
    }
}
