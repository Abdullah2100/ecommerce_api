using Microsoft.AspNetCore.Mvc;

using api.application.Result;

public interface IPaymentServices
{
    Task<Boolean> IsSuccessFullPayment(string id);
}