using api.domain.entity;

namespace data.Interface;

public interface IOrderProductVariant
{
    void Add(ICollection<OrderProductsVariant> entities);
}