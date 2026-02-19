using Application;
using Application.Helper;
using Application.Interfaces;
using Application.Services.FacilityServices;
using Domain.Repositories;
using FluentValidation;
using HotelDemo.Persistence;
using HotelDemo.ValidationFilters;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Application;
using Infrastructure;
using AutoMapper;
using Presentation.MappingProfiles.User;
using Application.Dtos.User;
using Presentation.ViewModels.User;
using Presentation.Validator;
using System.Reflection;
using Hangfire;

using Application.Services;
using Infrastructure.BackgroundServices;
using Infrastructure.Services;

namespace Presentation.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {


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
            services.AddValidatorsFromAssemblies(new[]
            {
                 typeof(Program).Assembly,
                 typeof(IApplicationMarker).Assembly,
                 typeof(IPresentationMarker).Assembly
            });

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            services.AddAuthentication(
                opt => opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme
                )
            .AddJwtBearer(
            opt =>
            {
                var jwtsettings = configuration.GetSection("Jwt").Get<JwtSettings>();
                var key = Encoding.ASCII.GetBytes(jwtsettings.Key);
                opt.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {

                    ValidIssuer = jwtsettings.Issuer,
                    ValidAudience = jwtsettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                };

            });

            services.AddInfrastructure();
            services.AddApplication();

            services.AddAutoMapper(
            typeof(IApplicationMarker).Assembly,
            typeof(IPresentationMarker).Assembly
            );
           
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);


            services.AddHangfire(config => config.UseSqlServerStorage(connectionString));


            services.AddHangfireServer();


           services.AddScoped<IBackgroundJobService, HangfireJobService>();


            services.AddScoped<IStripePaymentService, StripePaymentService>();
            services.AddHttpContextAccessor();

            services.AddScoped<CurrentUser>();
        }
    }
}
