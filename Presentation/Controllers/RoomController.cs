using Application.Dtos.Room;
using Application.Interfaces;
using Application.Services;
using AutoMapper;
using FluentValidation;
using HotelDemo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModels.Room;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;
        private readonly IValidator<CreateRoomRequestViewModel> _createRoomValidator;
        private readonly IValidator<UpdateRoomRequestViewModel> _updateRoomValidator;
        private readonly IValidator<RoomFilterRequestViewModel> _roomFilterValidator;
        public RoomController(IRoomService roomService,IMapper mapper, IValidator<CreateRoomRequestViewModel> createRoomValidator ,IValidator<UpdateRoomRequestViewModel> updateRoomValidator, IValidator<RoomFilterRequestViewModel> roomFilterValidator)
        {
            _roomService = roomService;
            _mapper = mapper;
            _createRoomValidator = createRoomValidator;
            _updateRoomValidator = updateRoomValidator;
            _roomFilterValidator = roomFilterValidator;
        }


        [HttpGet("")]

        public async Task<ResponseViewModel<IEnumerable<GetRoomResponseViewModel>>> GetAllRooms()
        {
            var result = await _roomService.GetAllRooms();
            if (!result.IsSuccess)
            {
                return ResponseViewModel<IEnumerable<GetRoomResponseViewModel>>.Fail(result.ErrorCode, result.Message);
            }
            var roomsViewModel = _mapper.Map<IEnumerable<GetRoomResponseViewModel>>(result.Data);
            var response = ResponseViewModel<IEnumerable<GetRoomResponseViewModel>>.Success(roomsViewModel);
            return response;
        }

        [HttpGet("{Id}")]
        public async Task<ResponseViewModel<GetRoomResponseViewModel>> GetRoomById(Guid Id)
        {
            var result = await _roomService.GetRoomById(Id);
            if (!result.IsSuccess)
            {
                return ResponseViewModel<GetRoomResponseViewModel>.Fail(result.ErrorCode, result.Message);
            }
            var roomViewModel = _mapper.Map<GetRoomResponseViewModel>(result.Data);
            var response = ResponseViewModel<GetRoomResponseViewModel>.Success(roomViewModel);
            return response;

        }

        [HttpPost("")]
        public async Task<ResponseViewModel<bool>> CreateRoom([FromBody] CreateRoomRequestViewModel dto)
        {
            var validator = _createRoomValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseViewModel<bool>.ValidationFail(validator);

            var createRoomDto = _mapper.Map<CreateRoomRequestDto>(dto);
            var result = await _roomService.CreateRoom(createRoomDto);
            if (!result.IsSuccess)
            {
                return ResponseViewModel<bool>.Fail(result.ErrorCode, result.Message);
            }
            return ResponseViewModel<bool>.Success(true, "Room created successfully");


        }

        [HttpPut("{Id}")]
        public async Task<ResponseViewModel<bool>> UpdateRoom(Guid Id, [FromBody] UpdateRoomRequestViewModel dto)
        {
            var validator = _updateRoomValidator.Validate(dto);
            if (!validator.IsValid)
                return ResponseViewModel<bool>.ValidationFail(validator);
            var updateRoomDto = _mapper.Map<UpdateRoomRequestDto>(dto);
            var result = await _roomService.UpdateRoom(Id, updateRoomDto);
            if (!result.IsSuccess)
            {
                return ResponseViewModel<bool>.Fail(result.ErrorCode, result.Message);
            }
            return ResponseViewModel<bool>.Success(true, "Room updated successfully");
        }

        [HttpDelete("{Id}")]
        public async Task<ResponseViewModel<bool>> DeleteRoom(Guid Id)
        {
            var result = await _roomService.DeleteRoom(Id);
            if (!result.IsSuccess)
            {
                return ResponseViewModel<bool>.Fail(result.ErrorCode, result.Message);
            }
            return ResponseViewModel<bool>.Success(true, "Room deleted successfully");
        }

        [HttpGet("filter")]
        public async Task<ResponseViewModel<PaginatedListResponseViewModel<GetRoomResponseViewModel>>> GetRoomsByFilter([FromQuery] RoomFilterRequestViewModel filterDto)
        {
            var validator = _roomFilterValidator.Validate(filterDto);
            if (!validator.IsValid)
                return ResponseViewModel<PaginatedListResponseViewModel<GetRoomResponseViewModel>>.ValidationFail(validator);
            var filterRequestDto = _mapper.Map<RoomFilterRequestDto>(filterDto);
            var result = await _roomService.GetRoomsByFilter(filterRequestDto);
            if (!result.IsSuccess)
            {
                return ResponseViewModel<PaginatedListResponseViewModel<GetRoomResponseViewModel>>.Fail(result.ErrorCode, result.Message);
            }
            var roomsViewModel = _mapper.Map<IEnumerable<GetRoomResponseViewModel>>(result.Data.Items);
            var paginatedResponse = new PaginatedListResponseViewModel<GetRoomResponseViewModel>(
            roomsViewModel,
            result.Data.PageNumber,
            result.Data.TotalPages,
            filterDto.PageSize
        );


            return ResponseViewModel<PaginatedListResponseViewModel<GetRoomResponseViewModel>>.Success(paginatedResponse);


        }
    }
}

