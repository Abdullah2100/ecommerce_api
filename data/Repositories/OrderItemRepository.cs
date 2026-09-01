
using api.application;
using api.domain.entity;
using data.Interface;
using data.util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="OrderItem"/> entities.
/// </summary>
/// <param name="context">The database context used for data access.</param>
/// <param name="logger">The logger used to log generated SQL queries.</param>
public class OrderItemRepository(
    AppDbContext context,
    ILogger<OrderItemRepository> logger
) : IOrderItemRepository
{
    /// <summary>
    /// Retrieves a paged collection of order items for a specific store.
    /// Includes related Order, Product, OrderProductsVariants, and Store data.
    /// Filters items where the associated Order Status is greater than 1.
    /// </summary>
    /// <param name="storeId">The unique identifier of the store.</param>
    /// <param name="pageNum">The page number to retrieve (1-indexed).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning a collection
    /// of order items.
    /// </returns>
    public async Task<ICollection<OrderItem>> GetOrderItems(
        Guid storeId,
        int pageNum,
        int pageSize
    )
    {
        var query = context.OrderItems
            .Include(oi => oi.Order)
            .Include(oi => oi.Product)
            .Include(oi => oi.OrderProductsVariants)
            .Include(oi => oi.Store)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(o => o.StoreId == storeId && ((int)o.Order.Status) > 1)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .OrderDescending();

        ClsUtil.logSql<OrderItemRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific order item by its identifier and store identifier.
    /// Includes related Product, OrderProductsVariants, and Store data.
    /// </summary>
    /// <param name="id">The unique identifier of the order item.</param>
    /// <param name="storeId">The unique identifier of the store.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning the order item
    /// or <c>null</c> if not found.
    /// </returns>
    public async Task<OrderItem?> GetOrderItem(Guid id, Guid storeId)
    {
        var query = context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.OrderProductsVariants)
            .Include(oi => oi.Store)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(o => o.Id == id && o.StoreId == storeId);

        ClsUtil.logSql<OrderItemRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Retrieves a specific order item by its identifier.
    /// Includes related Product, OrderProductsVariants, Store, and Order data.
    /// </summary>
    /// <param name="id">The unique identifier of the order item.</param>
    /// <returns>
    /// A task representing the asynchronous operation, returning the order item
    /// or <c>null</c> if not found.
    /// </returns>
    public async Task<OrderItem?> GetOrderItem(Guid id)
    {
        var query = context.OrderItems
            .Include(oi => oi.Product)
            .Include(oi => oi.OrderProductsVariants)
            .Include(oi => oi.Store)
            .Include(oi => oi.Order)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(o => o.Id == id);

        ClsUtil.logSql<OrderItemRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Adds a new order item to the database context.
    /// </summary>
    /// <param name="entity">The order item entity to add.</param>
    /// <remarks>
    /// The changes are not saved to the database until the DbContext is persisted.
    /// </remarks>
    public void Add(OrderItem entity)
    {
        context.OrderItems.Add(entity);
    }

    /// <summary>
    /// Updates an existing order item in the database context.
    /// </summary>
    /// <param name="entity">The order item entity with updated values.</param>
    /// <remarks>
    /// The changes are not saved to the database until the DbContext is persisted.
    /// </remarks>
    public void Update(OrderItem entity)
    {
        context.Update(entity);
    }
}
