using api.domain.entity;

namespace data.Interface;

public interface IPaymentTypeRepository : IRepository<PaymentType>
{
    public Task<PaymentType?> GetPaymentTypeGetPayment(Guid id);
    public Task<ICollection<PaymentType>> GetPaymentTypes(int pageNum, int pageSie);
    public Task<bool> IsExistPaymentType(string name, Guid id);
    public Task<bool> IsExistPaymentType(Guid id);
}