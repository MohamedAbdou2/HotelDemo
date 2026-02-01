using Application.Dtos.Room;
using Application.Interfaces;
using AutoMapper;
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
        public RoomController(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService;
            _mapper = mapper;
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
        public async Task<ResponseViewModel<PaginatedListResponseViewModel<IEnumerable<GetRoomResponseViewModel>>>> GetRoomsByFilter([FromQuery] RoomFilterRequestViewModel filterDto)
        {
            var filterRequestDto = _mapper.Map<RoomFilterRequestDto>(filterDto);
            var result = await _roomService.GetRoomsByFilter(filterRequestDto);
            if (!result.IsSuccess)
            {
                return ResponseViewModel<PaginatedListResponseViewModel<IEnumerable<GetRoomResponseViewModel>>>.Fail(result.ErrorCode, result.Message);
            }
            var roomsViewModel = _mapper.Map<IEnumerable<GetRoomResponseViewModel>>(result.Data.Items);
            var paginatedResponse = await PaginatedListResponseViewModel<IEnumerable<GetRoomResponseViewModel>>.CreateAsync(

                (IQueryable<IEnumerable<GetRoomResponseViewModel>>)roomsViewModel,
                filterDto.PageNumber,
                filterDto.PageSize

                );
            return ResponseViewModel<PaginatedListResponseViewModel<IEnumerable<GetRoomResponseViewModel>>>.Success(paginatedResponse);


        }
    }
}
