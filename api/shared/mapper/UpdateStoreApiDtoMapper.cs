using api.Request;
using data.dto.Request;
using api.util;

namespace api.shared.mapper;

public static class UpdateStoreApiDtoMapper
{
    public static UpdateStoreDto ToDto(this UpdateStoreApiDto data)
    {
        return new UpdateStoreDto
        {
            Name = data.Name,
            WallpaperImage = data.WallpaperImage is null ? null : data.WallpaperImage.ToBytes(),
            SmallImage = data.SmallImage is null ? null : data.SmallImage.ToBytes(),
            Longitude = data.Longitude,
            Latitude = data.Latitude,
        };
    }
}
