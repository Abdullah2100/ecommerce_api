using api.application;
using api.Presentation.dto.Request;
using data.dto.Request;
using data.dto.Response;

namespace business.Services.Interface;

public interface IDeliveryServices
{
    Task<Result> Login(LoginDto loginDto);

    Task<Result> CreateDelivery(Guid userId, CreateDeliveryDto deliveryDto,string rootPath);
    Task<Result> UpdateDeliveryStatus(Guid id, bool status);

    Task<Result> GetDelivery(Guid id);

    Task<Result> GetDeliveries(
        Guid belongToId,
        int pageNumber,
        int pageSize);

    Task<Result> UpdateDelivery(UpdateDeliveryDto deliveryDto, Guid id,string rootPath);
}