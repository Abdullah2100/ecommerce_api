using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Interface;

public interface IVariantServices
{
    Task<IActionResult> CreateVariant(CreateVariantDto variantDto, Guid adminId);
    Task<IActionResult> UpdateVariant(UpdateVariantDto variantDto, Guid adminId);
    Task<IActionResult> DeleteVariant(Guid variantId, Guid adminId);
    Task<IActionResult> GetVariants(int page, int pageSize);
    Task<IActionResult> GetVariantPage(Guid adminId, int variantPerPage);
}