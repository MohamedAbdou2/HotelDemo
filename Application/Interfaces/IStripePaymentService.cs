namespace Application.Interfaces
{
    public interface IStripePaymentService
    {
        Task<string> CreatePaymentSessionAsync(Guid paymentId, decimal amount, string currency = "usd");
        Task<(bool isSuccessful, string? sessionId, string? paymentIntentId)> GetSessionStatusAsync(Guid paymentId);
        Task<bool> VerifyPaymentAsync(string sessionId);
        Task<(bool isValid, string? eventType, Guid? paymentId, string? paymentIntentId)> ProcessWebhookAsync(string json, string stripeSignature);
        Task<bool> ProcessRefundAsync(string paymentIntentId, decimal? amount = null);
    }
}
