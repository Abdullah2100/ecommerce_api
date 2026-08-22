using api.Request;
using data.dto.Request;
using api.util;

namespace api.shared.mapper;

public static class UpdateUserInfoApiDtoMapper
{
    public static UpdateUserInfoDto ToDto(this UpdateUserInfoApiDto data)
    {
        return new UpdateUserInfoDto
        {
            Name = data.Name,
            Phone = data.Phone,
            Thumbnail = data.Thumbnail is null ? null : data.Thumbnail.ToBytes(),
            Password = data.Password,
            NewPassword = data.NewPassword,
        };
    }
}
