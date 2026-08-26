using api.application;
using api.domain.entity;
using data.Interface;
using data.util;

namespace data.Repositories;

public class OrderProductVariantRepository(AppDbContext context) : IOrderProductVariant
{
    public void Add(ICollection<OrderProductsVariant> entities)
    {
        foreach (var entity in entities)
        {
            context.OrdersProductsVarients.Add(new OrderProductsVariant()
            {
                Id = ClsUtil.GenerateGuid(),
                OrderItemId = entity.OrderItemId,
                ProductVariantId = entity.ProductVariantId,
            });
        }
    }
}