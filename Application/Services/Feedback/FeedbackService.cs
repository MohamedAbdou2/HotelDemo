using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos.Feedback;
using Application.Helper;
using Application.Interfaces;

namespace Application.Services.Feedback
{
    public class FeedbackService:IFeedbackService
    {
        private readonly CurrentUser currentUser;

        public FeedbackService(CurrentUser currentUser )
        {
            this.currentUser = currentUser;
        }

        public Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<FeedbackDto?> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<List<FeedbackDto>> GetByReservationIdAsync(Guid reservationId)
        {
            throw new NotImplementedException();
        }

        public Task<FeedbackDto> UpdateAsync(Guid id, UpdateFeedbackDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
