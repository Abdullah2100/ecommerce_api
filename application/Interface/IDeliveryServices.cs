using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IDeliveryServices
{
    Task<AuthDto?> Login(LoginDto loginDto);
    
    Task<DeliveryDto?> CreateDelivery(Guid userId,CreateDeliveryDto deliveryDto);
    Task<DeliveryDto?> UpdateDeliveryStatus(Guid id,bool status);
    
    Task<DeliveryDto?> GetDelivery(Guid id);
    
    Task<List<DeliveryDto>> GetDeliveries(
        Guid belongToId, 
        int pageNumber, 
        int pageSize);
    Task<DeliveryDto> UpdateDelivery(UpdateDeliveryDto deliveryDto,Guid id);
    
}