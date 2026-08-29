using api.application;
using api.domain.entity;
using data.Interface;
using data.util;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="OrderProductsVariant"/> entities.
/// This class provides methods to persist product variants associated with order items.
/// </summary>
/// <param name="context">The <see cref="AppDbContext"/> used for database operations.</param>
public class OrderProductVariantRepository(AppDbContext context) : IOrderProductVariant
{
    /// <summary>
    /// Adds a collection of <see cref="OrderProductsVariant"/> entities to the database context.
    /// Each entity is initialized with a new unique identifier using <see cref="ClsUtil.GenerateGuid"/>.
    /// </summary>
    /// <param name="entities">The collection of variants to add to the order.</param>
    /// <remarks>
    /// This method performs a bulk addition of variant records. Note that changes
    /// are only tracked by the context and require a subsequent call to SaveChanges
    /// to be persisted in the database.
    /// </remarks>
    public void Add(ICollection<OrderProductsVariant> entities)
    {
        foreach (var entity in entities)
        {
            context.OrdersProductsVariant.Add(new OrderProductsVariant()
            {
                Id = ClsUtil.GenerateGuid(),
                OrderItemId = entity.OrderItemId,
                ProductVariantId = entity.ProductVariantId,
            });
        }
    }
}