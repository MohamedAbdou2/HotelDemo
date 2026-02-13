using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Feedback;

namespace Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto);
        Task<FeedbackDto> UpdateAsync(Guid id, UpdateFeedbackDto dto);
        Task DeleteAsync(Guid id);
        Task<FeedbackDto?> GetByIdAsync(Guid id);
        Task<List<FeedbackDto>> GetByReservationIdAsync(Guid reservationId);
    }
}
