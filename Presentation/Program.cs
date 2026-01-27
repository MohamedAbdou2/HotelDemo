using Application;
using Application.Interfaces;
using FluentValidation;
using HotelDemo.Helper;
using HotelDemo.Persistence;
using Infrastructure;
using Infrastructure.DataSeeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
        


var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpContextAccessor();

builder.Services.AddInfrastructure();

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddApplication();




// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

builder.Services.AddAuthentication(opt => opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(
    opt =>
    {
        var jwtsettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>()
            ?? throw new InvalidOperationException("JWT configuration section 'Jwt' not found or invalid.");
        
        if (string.IsNullOrEmpty(jwtsettings.Key))
            throw new InvalidOperationException("JWT Key is not configured.");
        
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
var app = builder.Build();

await DataSeeder.SeedData(app.Services);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
