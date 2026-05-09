using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;


public interface IPaymentTypeServices
{
    public Task<PaymentTypeDto?> Create(CreatePaymentTypeDto paymentTypeDto,Guid adminId);
    public Task<PaymentTypeDto?> Update(UpdatePaymentTypeDto paymentTypeDto, Guid adminId);
    public Task<List<PaymentTypeDto>?> GetPaymentTypes(int pageNum, int pageSie = 25);
}