using Microsoft.AspNetCore.Mvc;
using api.application;
using data.dto.Request;

namespace api.application.Services.Interface;

public interface IVariantServices
{
    Task<Result> CreateVariant(CreateVariantDto variantDto, Guid adminId);
    Task<Result> UpdateVariant(UpdateVariantDto variantDto, Guid adminId);
    Task<Result> DeleteVariant(Guid variantId, Guid adminId);
    Task<Result> GetVariants(int page, int pageSize);
    Task<Result> GetVariantPage(Guid adminId, int variantPerPage);
}