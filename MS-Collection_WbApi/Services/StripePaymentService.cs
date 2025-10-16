using Stripe;

public class StripePaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;

    public StripePaymentService(IConfiguration configuration)
    {
        _configuration = configuration;
        StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
    }

    public async Task<PaymentIntentResponse> CreatePaymentIntentAsync(PaymentRequest request)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = request.Amount,
            Currency = "usd",
            PaymentMethodTypes = new List<string> { "card" }
        };

        var service = new PaymentIntentService();
        var intent = await service.CreateAsync(options);

        return new PaymentIntentResponse
        {
            ClientSecret = intent.ClientSecret
        };
    }
}