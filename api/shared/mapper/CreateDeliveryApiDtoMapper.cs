using api.Request;
using api.Presentation.dto.Request;
using api.util;
using data.dto.Request;

namespace api.shared.mapper;

public static class CreateDeliveryApiDtoMapper
{
    public static CreateDeliveryDto ToBusinessDto(this CreateDeliveryApiDto data)
    {
        return new CreateDeliveryDto
        {
            UserId = data.UserId,
            Thumbnail = data.Thumbnail is null ? null : data.Thumbnail.ToBytes(),
        };
    }
}
