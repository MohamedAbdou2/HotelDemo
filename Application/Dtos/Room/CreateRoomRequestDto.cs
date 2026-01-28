namespace Application.Dtos.Room
{
    public class CreateRoomRequestDto
    {
        public string RoomNumber { get; set; } = null!;
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; } = true;
        public int RoomTypeId { get; set; }
        public IEnumerable<string> RoomPictures { get; set; } = new List<string>();
    }
}
