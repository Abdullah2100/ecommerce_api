using api.Request;
using data.dto.Request;

namespace api.shared.mapper;

public static class CreateGeneralSettingApiDtoMapper
{
    public static CreateGeneralSettingDto ToDto(this CreateGeneralSettingApiDto data)
    {
        return new CreateGeneralSettingDto
        {
            Id = data.Id,
            Name = data.Name,
            Value = data.Value,
        };
    }
}
