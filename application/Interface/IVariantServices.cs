using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;

namespace api.application.Interface;

public interface IVariantServices
{
   Task<VariantDto?> CreateVariant(CreateVariantDto variantDto,Guid adminId); 
   Task<VariantDto?> UpdateVariant(UpdateVariantDto variantDto,Guid adminId); 
   Task<bool> DeleteVariant(Guid vairantId,Guid adminId); 
   Task<List<VariantDto>> GetVariants(int page,int pageSize);
   Task<int?> GetVariantPage(Guid adminId, int variantPerPage);
}