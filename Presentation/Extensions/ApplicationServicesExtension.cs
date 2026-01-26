using Application.Validator;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using HotelDemo.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblies(new[]
            {
                 typeof(Program).Assembly,
                 typeof(IApplicationMarker).Assembly
            });
            // AutoMapper - scans assembly for all Profile classes
            services.AddAutoMapper(typeof(Profile).Assembly);


        }
    }
}
