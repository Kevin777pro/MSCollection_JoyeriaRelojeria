public interface IPaymentService
{
    Task<PaymentIntentResponse> CreatePaymentIntentAsync(PaymentRequest request);
}