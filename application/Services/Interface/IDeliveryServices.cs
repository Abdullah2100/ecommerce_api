using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Interface;

public interface IDeliveryServices
{
    Task<IActionResult> Login(LoginDto loginDto);

    Task<IActionResult> CreateDelivery(Guid userId, CreateDeliveryDto deliveryDto);
    Task<IActionResult> UpdateDeliveryStatus(Guid id, bool status);

    Task<IActionResult> GetDelivery(Guid id);

    Task<IActionResult> GetDeliveries(
        Guid belongToId,
        int pageNumber,
        int pageSize);

    Task<IActionResult> UpdateDelivery(UpdateDeliveryDto deliveryDto, Guid id);
}