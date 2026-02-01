using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;

using Hangfire;
namespace Infrastructure.BackgroundServices
{
    

    public class HangfireJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _jobClient;

        public HangfireJobService(IBackgroundJobClient jobClient)
        {
            _jobClient = jobClient;
        }

        public void ScheduleReservationCancellation(Guid reservationId, TimeSpan delay)
        {
            _jobClient.Schedule<IReservationServices>(
                service => service.CheckAndCancelReservation(reservationId),
                delay);
        }
    }
}
