using api.application.Services.Interface;

namespace business.Services.Implement;

public class PaymentServices(IPaymentServices paymentServices)
{
    public async Task<Boolean> IsValidatePayment(string id)
    {
        return await paymentServices.IsSuccessFullPayment(id);
    }
}