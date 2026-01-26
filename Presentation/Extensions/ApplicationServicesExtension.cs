using Application.Validator;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using HotelDemo.Persistence;
using HotelDemo.ValidationFilters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Presentation.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static void AddApplicationServices(this IServiceCollection services , IConfiguration configuration)
        {// Add services to the container.

            services.AddControllers(options =>
            {
                options.Filters.Add<UnifiedValidationFilter>();
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            
            var connectionString = configuration.GetConnectionString("DefaultConnection") ??
                           throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddHttpContextAccessor();
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
