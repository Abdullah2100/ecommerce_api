using api.application;
using api.domain.entity;
using data.dto.Request;
using data.Interface;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="Order"/> entities.
/// Handles complex queries, order item tracking, currency conversion, and delivery assignments.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class OrderRepository(AppDbContext context)
    : IOrderRepository
{
    /// <summary>
    /// Retrieves a paged collection of orders for a specific user.
    /// Includes payment types, user details, and order items with product/store info using split queries.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="pageNum">The page number to retrieve (1-indexed).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of orders.</returns>
    public async Task<ICollection<Order>> GetOrders(
        Guid userId,
        int pageNum,
        int pageSize
    )
    {
        var orders = await context.Orders
            .Include(o => o.PaymentType)
            .Include(o => o.User)
            .Include(o => o.Items)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .OrderDescending()
            .ToListAsync();

        foreach (var order in orders)
        {
            order.Items = await context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Include(oi => oi.Store)
                .AsSplitQuery()
                .Where(oi => oi.OrderId == order.Id)
                .ToListAsync();
        }

        return orders;
    }

    /// <summary>
    /// Retrieves a paged collection of all orders in the system.
    /// Includes detailed store information and addresses for each order item.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="length">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of orders.</returns>
    public async Task<ICollection<Order>> GetOrders(int page, int length)
    {
        var orders = await context.Orders
            .Include(o => o.PaymentType)
            .Include(o => o.User)
            .Include(o => o.Items)
            .AsSplitQuery()
            .AsNoTracking()
            .Skip((page - 1) * length)
            .Take(length)
            .OrderDescending()
            .ToListAsync();

        foreach (var order in orders)
        {
            order.Items = await context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Include(oi => oi.Store)
                .AsSplitQuery()
                .Where(oi => oi.OrderId == order.Id)
                .Select(it => new OrderItem
                {
                    Id = it.Id,
                    OrderId = it.OrderId,
                    ProductId = it.ProductId,
                    Price = it.Price,
                    Quantity = it.Quantity,
                    StoreId = it.StoreId,
                    Order = it.Order,
                    Store = new Store
                    {
                        Id = it.Store.Id,
                        Name = it.Store.Name,
                        WallpaperImage = "",
                        SmallImage = "",
                        IsBlock = it.Store.IsBlock,
                        UserId = it.Store.UserId,
                        Addresses = context
                            .Address
                            .AsNoTracking()
                            .Where(ad => ad.OwnerId == it.Store.Id)
                            .ToList()
                    },
                    Product = it.Product,
                    OrderProductsVariants = it.OrderProductsVariants,
                    Status = it.Status
                })
                .ToListAsync();
        }

        return orders;
    }

    /// <summary>
    /// Retrieves a specified number of orders in a random order.
    /// </summary>
    /// <param name="randomNumber">The number of orders to retrieve.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of random orders.</returns>
    public async Task<ICollection<Order>> GetOrders(int randomNumber)
    {
        return await context
            .Orders
            .Include(o => o.PaymentType)
            .OrderBy(x => Guid.NewGuid())
            .Take(randomNumber)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific order by its identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>A task representing the asynchronous operation, returning the order or null if not found.</returns>
    public async Task<Order?> GetOrder(Guid id)
    {
        var order = await context.Orders
            .Include(o => o.PaymentType)
            .Include(o => o.User)
            .Include(o => o.Items)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id);

        if (order is null) return null;

        order.Items = await context.OrderItems
            .Include(oi => oi.Order)
            .Include(oi => oi.Product)
            .Include(oi => oi.Store)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(oi => oi.OrderId == order.Id)
            .ToListAsync();

        return order;
    }

    /// <summary>
    /// Retrieves a specific order belonging to a specific user.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <param name="userId">The unique identifier of the user owner.</param>
    /// <returns>A task representing the asynchronous operation, returning the order if it matches both IDs.</returns>
    public async Task<Order?> GetOrder(Guid id, Guid userId)
    {
        var order = await context.Orders
            .Include(o => o.PaymentType)
            .Include(o => o.User)
            .Include(o => o.Items)
            .AsSplitQuery()
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);

        if (order is null) return null;

        order.Items = await context.OrderItems
            .Include(oi => oi.Order)
            .Include(oi => oi.Product)
            .Include(oi => oi.Store)
            .AsSplitQuery()
            .Where(oi => oi.OrderId == order.Id)
            .ToListAsync();

        return order;
    }

    /// <summary>
    /// Gets the total count of orders in the database.
    /// </summary>
    /// <returns>A task representing the asynchronous operation, returning the total count.</returns>
    public async Task<int> GetOrders()
    {
        return await context.Orders.CountAsync();
    }

    /// <summary>
    /// Checks if an order exists with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier to check.</param>
    /// <returns>A task representing the asynchronous operation, returning true if found.</returns>
    public async Task<bool> IsExist(Guid id)
    {
        return await context.Orders
            .AsNoTracking()
            .AnyAsync(o => o.Id == id);
    }

    /// <summary>
    /// Determines if an order can still be cancelled based on the status of its items.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>A task representing the asynchronous operation, returning true if cancellation is possible.</returns>
    public async Task<bool> IsCanCancelOrder(Guid id)
    {
        return await context
            .OrderItems
            .AsNoTracking()
            .AnyAsync(i => i.OrderId == id && i.Status == EnOrderItemStatus.ReceivedByDelivery);
    }

    /// <summary>
    /// Validates if the total price provided by the client matches the calculated real price
    /// considering product prices, variants, and currency exchange rates.
    /// </summary>
    /// <param name="totalPrice">The total price to validate.</param>
    /// <param name="items">The items included in the order.</param>
    /// <param name="symbol">The target currency symbol for the calculation.</param>
    /// <returns>A task representing the asynchronous operation, returning true if the price is valid.</returns>
    public async Task<bool> IsValidTotalPrice(decimal totalPrice, ICollection<CreateOrderItemDto> items, string symbol)
    {
        bool isAmbiguous = false;
        decimal realPrice = 0;

        foreach (var item in items)
        {
            var product = await context.Products.FindAsync(item.ProductId);
            var currencies = await context.Currencies.ToListAsync();
            int variantPrice = 0;

            for (var i = 0; i < item.ProductVariant?.Count; i++)
            {
                var productVariantPrice = await context.ProductVariants.FirstOrDefaultAsync(p =>
                        p.ProductId == p.Id && p.Id == item.ProductVariant.ElementAt(i));

                if (productVariantPrice is null)
                {
                    isAmbiguous = true;
                    break;
                }

                variantPrice += productVariantPrice?.Percentage ?? product!.Price;
            }

            if (isAmbiguous) break;

            if (product?.Price != item.Price)
            {
                isAmbiguous = true;
                break;
            }

            realPrice += ConvertPriceFromCurrencyToAnother(
                ((variantPrice != 0 ? variantPrice : product.Price) * item.Quantity),
                product.Symbol,
                symbol,
                currencies);
        }

        return !isAmbiguous && realPrice == totalPrice;
    }

    /// <summary>
    /// Retrieves orders that have not yet been assigned to a delivery person.
    /// </summary>
    /// <param name="pageNum">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of unassigned orders.</returns>
    public async Task<ICollection<Order>> GetOrderNoBelongToAnyDelivery(int pageNum, int pageSize)
    {
        var orders = await context.Orders
                .Include(o => o.PaymentType)
                .Include(o => o.Items)
                .Include(o => o.User)
                .AsSplitQuery()
                .AsNoTracking()
                .Where(o => o.DeliveryId == null)
                .Skip((pageNum - 1) * pageSize)
                .Take(pageSize)
                .OrderDescending()
                .ToListAsync();

        foreach (var order in orders)
        {
            order.Items = await context.OrderItems
                .Include(it => it.Order)
                .Include(it => it.OrderProductsVariants)
                .Include(oi => oi.Product)
                .Include(oi => oi.Store)
                .AsSplitQuery()
                .Where(oi => oi.OrderId == order.Id)
                .Select(it => new OrderItem
                {
                    Id = it.Id,
                    OrderId = it.OrderId,
                    ProductId = it.ProductId,
                    Price = it.Price,
                    Quantity = it.Quantity,
                    StoreId = it.StoreId,
                    Order = it.Order,
                    Store = new Store
                    {
                        Id = it.Store.Id,
                        Name = it.Store.Name,
                        WallpaperImage = "",
                        SmallImage = "",
                        IsBlock = it.Store.IsBlock,
                        UserId = it.Store.UserId,
                        Addresses = context
                            .Address
                            .AsNoTracking()
                            .Where(ad => ad.OwnerId == it.Store.Id)
                            .ToList()
                    },
                    Product = it.Product,
                    OrderProductsVariants = it.OrderProductsVariants,
                    Status = it.Status
                })
                .ToListAsync();
        }

        return orders;
    }

    /// <summary>
    /// Retrieves orders currently assigned to a specific delivery person.
    /// </summary>
    /// <param name="deliveryId">The unique identifier of the delivery person.</param>
    /// <param name="pageNum">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of assigned orders.</returns>
    public async Task<ICollection<Order>> GetOrderBelongToDelivery(Guid deliveryId, int pageNum, int pageSize)
    {
        var orders = await context.Orders
            .Include(o => o.PaymentType)
            .Include(o => o.User)
            .Include(o => o.Items)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(o => o.DeliveryId == deliveryId)
            .Skip((pageNum - 1) * pageSize)
            .Take(pageSize)
            .OrderDescending()
            .ToListAsync();

        foreach (var order in orders)
        {
            order.Items = await context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Include(oi => oi.Store)
                .AsSplitQuery()
                .Where(oi => oi.OrderId == order.Id)
                .Select(it => new OrderItem
                {
                    Id = it.Id,
                    OrderId = it.OrderId,
                    ProductId = it.ProductId,
                    Price = it.Price,
                    Quantity = it.Quantity,
                    StoreId = it.StoreId,
                    Order = it.Order,
                    Store = new Store
                    {
                        Id = it.Store.Id,
                        Name = it.Store.Name,
                        WallpaperImage = "",
                        SmallImage = "",
                        IsBlock = it.Store.IsBlock,
                        UserId = it.Store.UserId,
                        Addresses = context
                            .Address
                            .AsNoTracking()
                            .Where(ad => ad.OwnerId == it.Store.Id)
                            .ToList()
                    },
                    Product = it.Product,
                    OrderProductsVariants = it.OrderProductsVariants,
                    Status = it.Status
                })
                .ToListAsync();
        }

        return orders;
    }

    /// <summary>
    /// Unassigns a specific order from a delivery person.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <param name="deliveryId">The unique identifier of the delivery person.</param>
    /// <exception cref="ArgumentNullException">Thrown when the order matching the criteria is not found.</exception>
    public void RemoveOrderFromDelivery(Guid id, Guid deliveryId)
    {
        Order? result = context
            .Orders
            .FirstOrDefault(o => o.Id == id && o.DeliveryId == deliveryId);

        if (result == null) throw new ArgumentNullException();
        result.DeliveryId = null;
    }

    /// <summary>
    /// Converts a price from one currency to another using provided currency rates.
    /// </summary>
    /// <param name="price">The original price value.</param>
    /// <param name="productSymbol">The currency symbol of the product's original price.</param>
    /// <param name="currentSymbol">The target currency symbol.</param>
    /// <param name="currencies">A collection of all available currencies and their exchange values.</param>
    /// <returns>The converted price value.</returns>
    public decimal ConvertPriceFromCurrencyToAnother(decimal price, string productSymbol, string currentSymbol,
        ICollection<Currency> currencies)
    {
        var currentCurrency = currencies.First(x => x.Symbol == currentSymbol);
        var productCurrency = currencies.First(x => x.Symbol == productSymbol);

        switch (currentCurrency.IsDefault && !productCurrency.IsDefault)
        {
            case true:
                return price / (productCurrency.Value);
            default:
            {
                switch (currentCurrency == productCurrency)
                {
                    case true: return price;
                    default:
                    {
                        return (price / productCurrency.Value) * currentCurrency.Value;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tracks a new order entity to be added to the database.
    /// </summary>
    /// <param name="entity">The order entity to add.</param>
    public void Add(Order entity)
    {
        context.Orders.Add(entity);
    }

    /// <summary>
    /// Updates an existing order entity in the database context.
    /// </summary>
    /// <param name="entity">The order entity with updated values.</param>
    public void Update(Order entity)
    {
        context.Orders.Update(entity);
    }

    /// <summary>
    /// Deletes a specific order by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the order to delete.</param>
    public async Task Delete(Guid id)
    {
        var orders = await context.Orders.Where(o => o.Id == id).ToListAsync();
        if (orders.Count == 0) return;
        context.Orders.RemoveRange(orders);
    }

    /// <summary>
    /// Deletes a collection of order entities from the database.
    /// </summary>
    /// <param name="orders">The collection of orders to remove.</param>
    public void Delete(ICollection<Order> orders)
    {
        context.Orders.RemoveRange(orders);
    }

    /// <summary>
    /// Checks if distance information was successfully saved for an order.
    /// If not, the order is deleted as it's considered invalid.
    /// </summary>
    /// <param name="id">The unique identifier of the order.</param>
    /// <returns>A task representing the asynchronous operation, returning true if distance is saved.</returns>
    public async Task<bool> IsSavedDistanceToOrder(Guid id)
    {
        var result = (await IsSavedDistance(id) == true ? 1 : 0);
        if (result == 0)
        {
           await Delete(id);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Executes the <c>fun_calculate_distance_between_user_and_stores</c> database function
    /// for a specific order.
    /// </summary>
    /// <param name="orderId">The unique identifier of the order.</param>
    /// <returns>A task representing the asynchronous operation, returning true if calculation was successful.</returns>
    private async Task<bool> IsSavedDistance(Guid orderId)
    {
        try
        {
            using (var command = context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = @"SELECT * FROM fun_calculate_distance_between_user_and_stores(@orderId)";
                command.Parameters.Add(new NpgsqlParameter("@orderId", orderId));
                await context.Database.OpenConnectionAsync();
                var result = await command.ExecuteScalarAsync();
                return (bool?)result == true;
            }
        }
        catch (System.Exception ex)
        {
            Console.WriteLine("Error from isSavedDistance " + ex);
            return false;
        }
    }
}