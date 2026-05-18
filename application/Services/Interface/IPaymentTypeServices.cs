using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Interface;

public interface IPaymentTypeServices
{
    public Task<IActionResult> Create(CreatePaymentTypeDto paymentTypeDto, Guid adminId);
    public Task<IActionResult> Update(UpdatePaymentTypeDto paymentTypeDto, Guid adminId);
    public Task<IActionResult> GetPaymentTypes(int pageNum, int pageSie = 25);
}