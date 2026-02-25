using Application.Dtos.Offers;
using Application.Interfaces;
using AutoMapper;
using FluentValidation;
using HotelDemo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModels.Offer;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[Controller]/[Action]")]
    public class OffersController : ControllerBase
    {
        private readonly IOfferService _offerService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateOfferViewModel> _createOfferValidator;
        private readonly IValidator<UpdateOfferViewModel> _updateOfferValidator;

        public OffersController(
            IOfferService offerService,
            IMapper mapper,
            IValidator<CreateOfferViewModel> createOfferValidator,
            IValidator<UpdateOfferViewModel> updateOfferValidator)
        {
            _offerService = offerService;
            _mapper = mapper;
            _createOfferValidator = createOfferValidator;
            _updateOfferValidator = updateOfferValidator;
        }

        [HttpPost]
        //[Authorize(Roles = "Staff,Admin")]
        public async Task<ResponseViewModel<object>> CreateOffer([FromBody] CreateOfferViewModel vm)
        {
            var validator = _createOfferValidator.Validate(vm);

            if (!validator.IsValid)
                return ResponseViewModel<object>.ValidationFail(validator);

            var createOfferDto = _mapper.Map<CreateOfferDto>(vm);

            var responseDto = await _offerService.CreateOfferAsync(createOfferDto);

            if (!responseDto.IsSuccess)
                return ResponseViewModel<object>.Fail(responseDto.ErrorCode, responseDto.Message);

            return ResponseViewModel<object>.Success(responseDto.Data, responseDto.Message);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ResponseViewModel<IEnumerable<OfferResponseViewModel>>> GetAll()
        {
            var resultDto = await _offerService.GetAllOffersAsync();

            var offerResponseVm = _mapper.Map<IEnumerable<OfferResponseViewModel>>(resultDto.Data);

            return ResponseViewModel<IEnumerable<OfferResponseViewModel>>.Success(offerResponseVm, resultDto.Message);
        }

        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ResponseViewModel<OfferResponseViewModel>> GetById(Guid id)
        {
            var responseDto = await _offerService.GetOfferByIdAsync(id);

            if (!responseDto.IsSuccess)
                return ResponseViewModel<OfferResponseViewModel>.Fail(responseDto.ErrorCode, responseDto.Message);

            var offerResponseVm = _mapper.Map<OfferResponseViewModel>(responseDto.Data);

            return ResponseViewModel<OfferResponseViewModel>.Success(offerResponseVm, responseDto.Message);
        }

        [HttpPut("{id:guid}")]
        //[Authorize(Roles = "Staff,Admin")]
        public async Task<ResponseViewModel<object>> UpdateOffer(Guid id, [FromBody] UpdateOfferViewModel vm)
        {
            var validator = _updateOfferValidator.Validate(vm);

            if (!validator.IsValid)
                return ResponseViewModel<object>.ValidationFail(validator);

            var updateOfferDto = _mapper.Map<UpdateOfferDto>(vm);

            var result = await _offerService.UpdateOfferAsync(id, updateOfferDto);

            if (!result.IsSuccess)
                return ResponseViewModel<object>.Fail(result.ErrorCode, result.Message);

            return ResponseViewModel<object>.Success(result.Data, result.Message);
        }

        [HttpDelete("{id:guid}")]
        //[Authorize(Roles = "Staff,Admin")]
        public async Task<ResponseViewModel<object>> Delete(Guid id)
        {
            var responseDto = await _offerService.DeleteOfferAsync(id);

            if (!responseDto.IsSuccess)
                return ResponseViewModel<object>.Fail(responseDto.ErrorCode, responseDto.Message);

            return ResponseViewModel<object>.Success(responseDto.Data, responseDto.Message);    
        }
    }
}
