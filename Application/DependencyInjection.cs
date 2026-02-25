using Application.Dtos.User;
using Application.Interfaces;
using Application.Services;
using Application.Services.FacilityServices;
using Application.Services.Feedback;
using Application.Services.OfferServices;
using Application.Services.PaymentServices;
using Application.Services.ReservationServices;
using Application.Services.RoomOffer;
using Application.Validator;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {

            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOfferService, OfferService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IReservationService, RerservationService>();
            services.AddScoped<IFacilityService, FacilityService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IRoomOfferService, RoomOfferService>();

            services.AddAutoMapper(cfg =>
            {
            }, Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);


            return services;
        }
    }
}
