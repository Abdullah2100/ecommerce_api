using api.Request;
using api.util;
using data.dto.Request;

namespace api.shared.mapper;

public static class UpdateCategoryApiDtoMapper
{
    public static UpdateCategoryDto ToDto(this UpdateCategoryApiDto data)
    {
        return new UpdateCategoryDto
        {
            Id = data.Id,
            Name = data.Name,
            Image = data.Image is null ? null : data.Image.ToBytes(),
        };
    }
}
