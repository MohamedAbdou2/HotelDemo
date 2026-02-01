using Application;
using Application.Interfaces;
using Application.Services.OfferServices;
using Application.Validator;
using Domain.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
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

namespace Presentation.Extensions
{
    public static class ApplicationServicesExtension
    {
        public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
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

            services.Configure<JwtSettings>(configuration.GetSection("Jwt"));

            services.AddAuthentication(opt => opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(
            opt =>
            {
                var jwtsettings = configuration.GetSection("Jwt").Get<JwtSettings>();
                var key = Encoding.UTF8.GetBytes(jwtsettings.Key);
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
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IReadOnlyRepository<>), typeof(ReadOnlyRepository<>));

            services.AddScoped<IOffers, OfferService>();

            services.AddApplication();
            //services.AddScoped<IOfferRepository, OfferRepository>();
            // AutoMapper - scans assembly for all Profile classes
            //services.AddAutoMapper(typeof(Profile).Assembly);
        /*    services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"))); // تأكد من اسم الـ Connection String

            // 2. إضافة الـ Server (المسؤول عن تنفيذ المهام في الخلفية)
            services.AddHangfireServer();*/
        }
    }
}
