namespace Application.Interfaces
{
    public interface IStripePaymentService
    {
        Task<string> CreatePaymentSessionAsync(Guid paymentId, decimal amount, string currency = "usd");
        Task<bool> VerifyPaymentAsync(string sessionId);
        Task<bool> ProcessRefundAsync(string paymentIntentId, decimal? amount = null);
    }
}
