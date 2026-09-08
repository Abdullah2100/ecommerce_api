using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface IPaymentTypeRepository : IRepository<PaymentType>
{
    public Task<PaymentType?> GetPaymentTypeGetPayment(Guid id);
    public Task<ICollection<PaymentTypeDto>> GetPaymentTypes(int pageNum, int pageSie,string url);
    public Task<bool> IsExistPaymentType(string name, Guid id);
}