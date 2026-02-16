using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Dtos;
using Application.Dtos.Feedback;

namespace Application.Interfaces
{
    public interface IFeedbackService
    {
        Task<ResponseDto<bool>> CreateAsync(CreateFeedbackDto dto);

        Task<ResponseDto<bool>> UpdateAsync(Guid id, UpdateFeedbackDto dto);
        Task<ResponseDto<bool>> DeleteAsync(Guid id);
        Task<ResponseDto<FeedbackDto>> GetByIdAsync(Guid id);

    }
}
