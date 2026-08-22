using api.Request;
using api.Presentation.dto.Request;
using api.util;
using data.dto.Request;

namespace api.shared.mapper;

public static class UpdateDeliveryApiDtoMapper
{
    public static UpdateDeliveryDto ToBusinessDto(this UpdateDeliveryApiDto data)
    {
        return new UpdateDeliveryDto
        {
            Thumbnail = data.Thumbnail is null ? null : data.Thumbnail.ToBytes(),
            Longitude = data.Longitude,
            Latitude = data.Latitude,
            Name = data.Name,
            Phone = data.Phone,
            Password = data.Password,
            NewPassword = data.NewPassword,
        };
    }
}
