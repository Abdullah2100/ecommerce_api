using api.application;
using data.dto.Request;

namespace business.Services.Interface;

public interface IPaymentTypeServices
{
    public Task<Result> Create(CreatePaymentTypeDto paymentTypeDto, Guid adminId,string rootPath);
    public Task<Result> Update(UpdatePaymentTypeDto paymentTypeDto, Guid adminId,string rootPath);
    public Task<Result> GetPaymentTypes(int pageNum, int pageSie = 25);
}