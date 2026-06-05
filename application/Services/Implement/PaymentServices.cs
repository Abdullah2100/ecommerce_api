using api.application.Services.Interface;

namespace api.application.Services.Implement;

public class PaymentServices(IPaymentServices paymentServices)
{
    public async Task<Boolean> IsValidatePayment(string id)
    {
        return await paymentServices.IsSuccessFullPayment(id);
    }
}