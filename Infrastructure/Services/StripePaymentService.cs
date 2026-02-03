using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Stripe;
using Stripe.Checkout;

namespace Infrastructure.Services
{
    public class StripePaymentService : IStripePaymentService
    {
        private readonly IConfiguration _configuration;

        public StripePaymentService(IConfiguration configuration)
        {
            _configuration = configuration;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        public async Task<string> CreatePaymentSessionAsync(Guid paymentId, decimal amount, string currency = "usd")
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = currency,
                            UnitAmount = (long)(amount * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = "Hotel Reservation Payment",
                                Description = "Payment for hotel room reservation"
                            }
                        },
                        Quantity = 1
                    }
                },
                Mode = "payment",
                SuccessUrl = $"{_configuration["AppUrl"]}/payment/success?paymentId={paymentId}",
                CancelUrl = $"{_configuration["AppUrl"]}/payment/cancel?paymentId={paymentId}",
                Metadata = new Dictionary<string, string>
                {
                    { "payment_id", paymentId.ToString() }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }

        public async Task<bool> VerifyPaymentAsync(string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            return session.PaymentStatus == "paid";
        }

        public async Task<bool> ProcessRefundAsync(string paymentIntentId, decimal? amount = null)
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = paymentIntentId
            };

            if (amount.HasValue)
                options.Amount = (long)(amount.Value * 100);

            var service = new RefundService();
            var refund = await service.CreateAsync(options);

            return refund.Status == "succeeded";
        }
    }
}
