using Domain.Models;

namespace Application.Dtos.Room;

public class RoomDetailsDto
{
    public Guid Id { get; set; }
    public string RoomNumber { get; set; } 
    public string Type { get; set; } 

}
