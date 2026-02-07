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
                SuccessUrl = $"{_configuration["AppUrl"]}/api/payment/success?paymentId={paymentId}",
                CancelUrl = $"{_configuration["AppUrl"]}/api/payment/cancel?paymentId={paymentId}",
                Metadata = new Dictionary<string, string>
                {
                    { "payment_id", paymentId.ToString() }
                }
            };

            var service = new SessionService();
            var session = await service.CreateAsync(options);

            return session.Url;
        }

        public async Task<(bool isSuccessful, string? sessionId, string? paymentIntentId)> GetSessionStatusAsync(Guid paymentId)
        {
            var service = new SessionService();
            var options = new SessionListOptions
            {
                Limit = 1
            };

            var sessions = await service.ListAsync(options);
            var session = sessions.Data.FirstOrDefault(s => 
                s.Metadata.ContainsKey("payment_id") && 
                s.Metadata["payment_id"] == paymentId.ToString());

            if (session == null)
                return (false, null, null);

            var isSuccessful = session.PaymentStatus == "paid";
            return (isSuccessful, session.Id, session.PaymentIntentId);
        }

        public async Task<bool> VerifyPaymentAsync(string sessionId)
        {
            var service = new SessionService();
            var session = await service.GetAsync(sessionId);

            return session.PaymentStatus == "paid";
        }

        public Task<(bool isValid, string? eventType, Guid? paymentId, string? paymentIntentId)> ProcessWebhookAsync(string json, string stripeSignature)
        {
            try
            {
                var webhookSecret = _configuration["Stripe:WebhookSecret"];
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, webhookSecret);

                if (stripeEvent.Type == "checkout.session.completed")
                {
                    var session = stripeEvent.Data.Object as Session;
                    if (session == null)
                        return Task.FromResult<(bool, string?, Guid?, string?)>((false, null, null, null));

                    if (session.Metadata.TryGetValue("payment_id", out var paymentIdStr) &&
                        Guid.TryParse(paymentIdStr, out var paymentId))
                    {
                        return Task.FromResult<(bool, string?, Guid?, string?)>((true, stripeEvent.Type, paymentId, session.PaymentIntentId));
                    }
                }

                return Task.FromResult<(bool, string?, Guid?, string?)>((true, stripeEvent.Type, null, null));
            }
            catch (StripeException)
            {
                return Task.FromResult<(bool, string?, Guid?, string?)>((false, null, null, null));
            }
        }
    }
}
