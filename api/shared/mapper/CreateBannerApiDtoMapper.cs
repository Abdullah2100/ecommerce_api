using api.Request;
using data.dto.Request;
using api.util;

namespace api.shared.mapper;

public static class CreateBannerApiDtoMapper
{
    public static CreateBannerDto ToBusinessDto(this CreateBannerApiDto data)
    {
        using var stream = new MemoryStream();
        data.Image.CopyTo(stream);
        return new CreateBannerDto(stream.ToArray(), data.EndAt);
    }
}
