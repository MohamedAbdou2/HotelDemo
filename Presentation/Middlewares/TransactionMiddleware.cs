using HotelDemo.Persistence;

namespace HotelDemo.Middlewares
{
    public class TransactionMiddleWare : IMiddleware
    {
        private readonly ApplicationDbContext _context;

        public TransactionMiddleWare(ApplicationDbContext context)
        {
            this._context = context;
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
