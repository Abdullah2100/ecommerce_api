using api.Request;
using api.util;
using data.dto.Request;

namespace api.shared.mapper;

public static  class ApiRequestMapper
{
    public static CreateBannerDto ToBusinessDto(this CreateBannerApiDto data)
    {
       
        return new CreateBannerDto(data.Image.ToBytes(), data.EndAt);
    }
}