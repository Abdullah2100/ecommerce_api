using api.application;
using api.domain.entity;
using data.dto.Response;
using data.Interface;
using data.util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace data.Repositories;

/// +<summary>
/// Repository implementation for managing <see cref="Delivery"/> entities and their associated data.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class DeliveryRepository(
    AppDbContext context,
    ILogger<DeliveryRepository> logger
) : IDeliveryRepository
{
    /// <summary>
    /// Adds a new delivery record to the database.
    /// </summary>
    /// <param name="entity">The delivery entity containing information to be stored.</param>
    public void Add(Delivery entity)
    {
        context.Deliveries.Add(new Delivery
        {
            DeviceToken = entity.DeviceToken,
            Id = entity.Id,
            CreatedAt = DateTime.Now,
            UserId = entity.UserId,
            Thumbnail = entity.Thumbnail,
            BelongTo = entity.BelongTo
        });
    }

    /// <summary>
    /// Updates an existing delivery record in the database.
    /// </summary>
    /// <param name="entity">The delivery entity with updated values.</param>
    public void Update(Delivery entity)
    {
        context.Deliveries.Update(new Delivery
        {
            DeviceToken = entity.DeviceToken,
            Id = entity.Id,
            CreatedAt = DateTime.Now,
            UserId = entity.UserId,
            Thumbnail = entity.Thumbnail,
        });
    }

    /// <summary>
    /// Toggles the blocked status of a delivery person by their ID.
    /// </summary>
    /// <param name="id">The unique identifier of the delivery entity.</param>
    /// <exception cref="ArgumentNullException">Thrown when the delivery entity is not found.</exception>
    public void Delete(Guid id)
    {
        var entity = context.Deliveries.Find(id);
        if (entity is null) throw new ArgumentNullException();
        entity.IsBlocked = !entity.IsBlocked;
    }

    /// <summary>
    /// Retrieves a delivery record by its ID, including the associated user and address.
    /// </summary>
    /// <param name="id">The unique identifier of the delivery entity.</param>
    /// <returns>A task representing the asynchronous operation, returning the delivery if found; otherwise, null.</returns>
    public async Task<Delivery?> GetDelivery(Guid id)
    {
        var query = context
            .Deliveries
            .Include(de => de.User)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(de => de.Id == id);

        ClsUtil.logSql<DeliveryRepository>(logger, query.ToQueryString());

        if (!await query.AnyAsync()) return null;

        var delivery = await query.FirstOrDefaultAsync();
        var addressSql = context.Address
            .AsNoTracking()
            .Where(ad => ad.OwnerId == (delivery!.Id));

        ClsUtil.logSql<DeliveryRepository>(logger, addressSql.ToQueryString());
        delivery?.Address = await addressSql.FirstOrDefaultAsync();

        return delivery;
    }

    /// <summary>
    /// Retrieves a delivery record associated with a specific User ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task representing the asynchronous operation, returning the delivery if found; otherwise, null.</returns>
    public async Task<Delivery?> GetDeliveryByUserId(Guid userId)
    {
        var query = (context
            .Deliveries
            .AsNoTracking()
            .Include(de => de.User)
            .Where(de => de.UserId == userId));

        if (!await query.AnyAsync()) return null;
        ClsUtil.logSql<DeliveryRepository>(logger, query.ToQueryString());

        var delivery = await query.FirstOrDefaultAsync();
        var addressSql = context.Address
            .AsNoTracking()
            .Where(ad => ad.OwnerId == delivery!.Id);

        ClsUtil.logSql<DeliveryRepository>(logger, addressSql.ToQueryString());
        delivery?.Address = await addressSql.FirstOrDefaultAsync();

        return delivery;
    }

    /// <summary>
    /// Retrieves a paged collection of deliveries that belong to a specific entity (e.g., a store or region).
    /// </summary>
    /// <param name="belongToId">The identifier the delivery belongs to.</param>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="size">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of deliveries.</returns>
    public async Task<ICollection<Delivery>?> GetDeliveriesByBelongTo(Guid belongToId, int page, int size)
    {
        var query = context
            .Deliveries
            .Include(de => de.User)
            .AsSplitQuery()
            .AsNoTracking()
            .Take(page)
            .Skip((page - 1) * size);
        if (!await query.AnyAsync()) return null;

        var deliveries = await query.ToListAsync();
        foreach (var delivery in deliveries)
        {
            var addressSql = context.Address
                .AsNoTracking()
                .Where(ad => ad.Id == delivery.Id);

            ClsUtil.logSql<DeliveryRepository>(logger, addressSql.ToQueryString());
            delivery?.Address = await addressSql.FirstOrDefaultAsync();
        }

        return deliveries;
    }

    /// <summary>
    /// Retrieves a paged collection of all delivery records.
    /// </summary>
    /// <param name="page">The page number to retrieve.</param>
    /// <param name="size">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of deliveries.</returns>
    public async Task<ICollection<Delivery>?> GetDeliveries(int page, int size)
    {
        var query = context
                .Deliveries
                 .Include(de => de.User)
                 .AsSplitQuery()
            .AsNoTracking()
            .Take(page)
                .Skip((page - 1) * size)
            ;

        if (!await query.AnyAsync()) return null;

        return await query.ToListAsync();        
    }

    /// <summary>
    /// Calculates the total number of pages available based on a specified page size.
    /// </summary>
    /// <param name="deliveryPerSize">The number of deliveries per page.</param>
    /// <returns>A task representing the asynchronous operation, returning the total page count.</returns>
    public async Task<int> GetDeliveriesPage(int deliveryPerSize)
    {
        var deliveriesCount = await context.Deliveries.CountAsync();
        if (deliveriesCount == 0) return 0;
        return (int)Math.Ceiling((decimal)(deliveriesCount) / deliveryPerSize);
    }

    /// <summary>
    /// Retrieves performance analysis for a specific delivery person by executing the <c>get_delivery_fee_info</c> database function.
    /// </summary>
    /// <param name="id">The unique identifier of the delivery person.</param>
    /// <returns>A task representing the asynchronous operation, returning a <see cref="DeliveryAnalyseDto"/> or null.</returns>
    public async Task<DeliveryAnalyseDto?> GetDeliveryAnalys(Guid id)
    {
        await using var cmd = context.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "SELECT * FROM get_delivery_fee_info(@deliveryId)";
        cmd.Parameters.Add(new NpgsqlParameter("@deliveryId", id));
        await context.Database.OpenConnectionAsync();
        DeliveryAnalyseDto? info = null;
        await using var reader = await cmd.ExecuteReaderAsync();
        if (!reader.HasRows) return info;
        if (await reader.ReadAsync())
        {
            info = new DeliveryAnalyseDto
            {
                DayFee = (decimal?)reader["dayFee"],
                WeekFee = (decimal?)reader["weekFee"],
                MonthFee = (decimal?)reader["monthFee"],
                DayOrder = (int)reader["dayorder"],
                WeekOrder = (int)reader["weekorder"]
            };
        }

        return info;
    }

    /// <summary>
    /// Checks if a delivery record exists for the given User ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to check.</param>
    /// <returns>A task representing the asynchronous operation, returning true if it exists; otherwise, false.</returns>
    public async Task<bool> IsExistByUserId(Guid userId)
    {
        var query =  context
            .Deliveries
            .AsNoTracking()
            .Where(de => de.UserId == userId);
        if (!await query.AnyAsync()) return false;

        return  await query.AnyAsync();
    }
}