using api.domain.entity;
using api.Presentation.dto;

namespace api.shared.mapper;

public static class ExperientialExtension
{
    extension(Variant variant)
    {
        public VariantDto ToDto()
        {
            return new VariantDto
            {
                Id = variant.Id,
                Name = variant.Name,

            };
        }
    }

    extension(UpdateVariantDto dto)
    {
        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(dto.Name?.Trim());

        }
    }
}
    
