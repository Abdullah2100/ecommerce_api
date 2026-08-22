using api.Request;
using data.dto.Request;

namespace api.shared.mapper;

public static class CreateStoreApiDtoMapper
{
    public static CreateStoreDto ToDto(this CreateStoreApiDto data)
    {
        using var wallpaperStream = new MemoryStream();
        data.WallpaperImage.CopyTo(wallpaperStream);

        using var smallImageStream = new MemoryStream();
        data.SmallImage.CopyTo(smallImageStream);

        return new CreateStoreDto
        {
            Name = data.Name,
            WallpaperImage = wallpaperStream.ToArray(),
            SmallImage = smallImageStream.ToArray(),
            Longitude = data.Longitude,
            Latitude = data.Latitude,
        };
    }
}
