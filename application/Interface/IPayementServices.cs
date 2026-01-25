namespace api.application.Interface;
using api.application.Result;

public interface IPaymentServices
{
    Task<Boolean> IsSuccessFullPayment(string id);
}