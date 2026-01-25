using api.application.Interface;
using api.application.Result;

namespace api.application.Services;

public class PaymentServices(IPaymentServices paymentServices)
{

    public async Task<Boolean> IsValidatePayment(string id)
    {
      return await paymentServices.IsSuccessFullPayment(id);
    }
}