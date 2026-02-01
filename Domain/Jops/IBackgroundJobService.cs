using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Jops
{
    public interface IBackgroundJobService
    {
       
        void ScheduleReservationCancellation(Guid reservationId, TimeSpan delay);
    }
}
