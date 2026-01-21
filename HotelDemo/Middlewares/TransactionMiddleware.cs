using HotelDemo.Persistence;

namespace HotelDemo.Middlewares
{
    public class TransactionMiddleWare : IMiddleware
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TransactionMiddleWare> logger;

        public TransactionMiddleWare(ApplicationDbContext context , ILogger<TransactionMiddleWare> logger)
        {
            this._context = context;
            this.logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                await next(context);
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
