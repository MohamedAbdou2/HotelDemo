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
            // Fix: skip transaction for read-only requests
            if (HttpMethods.IsGet(context.Request.Method) ||
                HttpMethods.IsHead(context.Request.Method) ||
                HttpMethods.IsOptions(context.Request.Method))
            {
                await next(context);
                return;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
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
