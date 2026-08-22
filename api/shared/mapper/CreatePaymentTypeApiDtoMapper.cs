using api.Request;
using data.dto.Request;
using api.util;

namespace api.shared.mapper;

public static class CreatePaymentTypeApiDtoMapper
{
    public static CreatePaymentTypeDto ToDto(this CreatePaymentTypeApiDto data)
    {
        return new CreatePaymentTypeDto
        {
            Name = data.Name,
            IsHashCheckOperation = data.IsHashCheckOperation,
            Thumbnail = data.Thumbnail.ToBytes(),
        };
    }
}
