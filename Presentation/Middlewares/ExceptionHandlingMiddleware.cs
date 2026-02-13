
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace HotelDemo.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var Error = new ProblemDetails();
                logger.LogError(ex, $"Somthing went wrong {ex.Message}");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                if (ex is DbUpdateConcurrencyException)
                {
                    Error = new ProblemDetails()
                    {
                        Status = StatusCodes.Status409Conflict,
                        Title = "Data is Updated please refrech to get updates",
                    };
                }
                else
                {
                    Error = new ProblemDetails()
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Title = "Internal Server Error",
                    };
                }
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(Error);

            }

        }
    }
}
