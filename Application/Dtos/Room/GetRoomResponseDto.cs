namespace Application.Dtos.Room
{
    public class GetRoomResponseDto
    {
        public Guid Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }

        public IEnumerable<string> RoomPictures { get; set; } = new List<string>();

    }
}
