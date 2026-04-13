using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IDeliveryServices
{
    Task<Result<AuthDto?>> Login(LoginDto loginDto);
    
    Task<Result<DeliveryDto?>> CreateDelivery(Guid userId,CreateDeliveryDto deliveryDto);
    Task<Result<DeliveryDto?>> UpdateDeliveryStatus(Guid id,bool status);
    
    Task<Result<DeliveryDto?>> GetDelivery(Guid id);
    
    Task<Result<List<DeliveryDto>>> GetDeliveries(
        Guid belongToId, 
        int pageNumber, 
        int pageSize);
    Task<Result<DeliveryDto>> UpdateDelivery(UpdateDeliveryDto deliveryDto,Guid id);
    
}