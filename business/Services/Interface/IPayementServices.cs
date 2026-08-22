namespace api.application.Services.Interface;

public interface IPaymentServices
{
    Task<Boolean> IsSuccessFullPayment(string id);
}