using api.application.Result;
using api.Presentation.dto;

namespace api.application.Interface;


public interface IPaymentTypeServices
{
    public Task<Result<PaymentTypeDto?>> Create(CreatePaymentTypeDto paymentTypeDto,Guid adminId);
    public Task<Result<PaymentTypeDto?>> Update(UpdatePaymentTypeDto paymentTypeDto, Guid adminId);
    public Task<Result<List<PaymentTypeDto>?>> GetPaymentTypes(int pageNum, int pageSie = 25);
}