using HotelDemo.Helper;
using HotelDemo.Persistence;
using HotelDemo.ValidationFilters;
using Infrastructure.DataSeeding;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.Extensions;
using System.Text;
using System.Text.Json.Serialization;
using Application.Interfaces;
using Application.Services.OfferServices;
using Domain.Repositories;
using Infrastructure.Repositories;
using AutoMapper;
using Application.MappingProfiles.Offer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
