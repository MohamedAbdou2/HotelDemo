using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Room
{
    public class UpdateRoomRequestDto
    {
        public string RoomNumber { get; set; } = null!;
        public int RoomTypeId { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
        public List<string> RoomPictures { get; set; } = new List<string>();
    }
}
