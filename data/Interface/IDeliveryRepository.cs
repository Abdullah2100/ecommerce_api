using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface IDeliveryRepository : IRepository<Delivery>
{
    Task<Delivery?> GetDelivery(Guid id);
    Task<Delivery?> GetDeliveryByUserId(Guid userId);
    Task<ICollection<DeliveryDto>> GetDeliveriesByBelongTo(Guid belongToId, int page, int size, string url);
    Task<List<string?>> GetDeliveries(int page, int size);
    Task<int> GetDeliveriesPage(int deliveryPerSize);


    Task<DeliveryAnalyseDto?> GetDeliveryAnalys(Guid id);

    Task<bool> IsExistByUserId(Guid userId);

    void Delete(Guid id);
}