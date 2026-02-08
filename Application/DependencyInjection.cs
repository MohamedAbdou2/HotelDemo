using Application.Dtos.User;
using Application.Interfaces;
using Application.Services;
using Application.Services.FacilityServices;
using Application.Services.OfferServices;
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
            services.AddScoped<IOffers, OfferService>();
            services.AddScoped<IFacilityService, FacilityService>();

            services.AddAutoMapper(cfg =>
            {
            }, Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);


            return services;
        }
    }
}
