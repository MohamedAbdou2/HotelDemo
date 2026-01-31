using Application.Helper;
using Application.Interfaces;
using Application.Services.OfferServices;
using Application.Validator;
using Domain.Repositories;
using FluentValidation;
using HotelDemo.Helper;
using HotelDemo.Persistence;
using HotelDemo.ValidationFilters;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

namespace Presentation.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {// Add services to the container.
            services.AddApplication();

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
            //services.AddFluentValidationAutoValidation();
            //services.AddValidatorsFromAssemblies(new[]
            //{
            //     typeof(Program).Assembly,
            //     typeof(IApplicationMarker).Assembly
            //});

            //services.AddScoped<IValidator<RegisterViewModel>, RegisterViewModelValidator>();
            //services.AddScoped<IValidator<LoginViewModel>, LoginViewModelValidator>();
            //services.AddScoped<IValidator<R>, LoginViewModelValidator>();
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
            //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            //services.AddScoped(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>));
            services.AddAuthorization();
            services.AddInfrastructure();


            //services.AddScoped<IOfferRepository, OfferRepository>();
            // AutoMapper - scans assembly for all Profile classes
            //services.AddAutoMapper(typeof(Profile).Assembly);           
            services.AddAutoMapper(cfg =>
            {
            }, Assembly.GetExecutingAssembly());
            services.AddValidatorsFromAssembly(typeof(Program).Assembly);
            services.AddScoped<CurrentUser>();
        }
    }
}
