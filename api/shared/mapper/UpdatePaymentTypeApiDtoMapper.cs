using api.Request;
using data.dto.Request;
using api.util;

namespace api.shared.mapper;

public static class UpdatePaymentTypeApiDtoMapper
{
    public static UpdatePaymentTypeDto ToDto(this UpdatePaymentTypeApiDto data)
    {
        return new UpdatePaymentTypeDto
        {
            Id = data.Id,
            Name = data.Name,
            IsHashCheckOperation = data.IsHashCheckOperation,
            Thumbnail = data.Thumbnail is null ? null : data.Thumbnail.ToBytes(),
        };
    }
}
