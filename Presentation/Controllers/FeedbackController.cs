using Application.Dtos.Feedback;
using Application.Interfaces;
using AutoMapper;
using Domain.Enums;
using FluentValidation;
using HotelDemo.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModels.Feedback;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IFeedbackService _feedbackService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateFeedbackViewModel> _createValidator;
        private readonly IValidator<UpdateFeedbackViewModel> _updateValidator;

        public FeedbackController(
            IFeedbackService feedbackService,
            IMapper mapper,
            IValidator<CreateFeedbackViewModel> createValidator,
            IValidator<UpdateFeedbackViewModel> updateValidator
          )
        {
            _feedbackService = feedbackService;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpPost("add-feedback")]
        public async Task<ResponseViewModel<bool>> Create(
         CreateFeedbackViewModel model)
        {
            var validationResult = await _createValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
                return ResponseViewModel<bool>
                    .ValidationFail(validationResult);

            var dto = _mapper.Map<CreateFeedbackDto>(model);

            var result = await _feedbackService.CreateAsync(dto);

            return result.IsSuccess? ResponseViewModel<bool>.Success(result.Data , result.Message)
                : ResponseViewModel<bool>.Fail(result.ErrorCode, result.Message);
        }

        [HttpPut("{id}")]
        public async Task<ResponseViewModel<bool>> Update(
           Guid id,
           UpdateFeedbackViewModel model)
        {
            var validationResult = await _updateValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
                return ResponseViewModel<bool>
                    .ValidationFail(validationResult);

            var dto = _mapper.Map<UpdateFeedbackDto>(model);

            var result = await _feedbackService.UpdateAsync(id, dto);

            return result.IsSuccess ? ResponseViewModel<bool>.Success(result.Data, result.Message)
                : ResponseViewModel<bool>.Fail(result.ErrorCode, result.Message); 
        }

        [HttpGet("{id}")]
        public async Task<ResponseViewModel<FeedbackViewModel>> GetById(Guid id)
        {
            var result = await _feedbackService.GetByIdAsync(id);

            if (result == null)
                return ResponseViewModel<FeedbackViewModel>
                    .Fail(ErrorCode.NotFound, "Feedback not found");

            var viewModel = _mapper.Map<FeedbackViewModel>(result);

            return ResponseViewModel<FeedbackViewModel>
                .Success(viewModel);
        }

        [HttpDelete("{id}")]
        public async Task<ResponseViewModel<bool>> Delete(Guid id)
        {
           var result =  await _feedbackService.DeleteAsync(id);

            return result.IsSuccess?  ResponseViewModel<bool>.Success(result.Data, result.Message)
                :ResponseViewModel<bool>.Fail(result.ErrorCode, result.Message);
        }
    }
}
