using api.domain.entity;
using api.Presentation.dto;
using api.Presentation.dto.Response;

namespace api.shared.mapper;

public static class BannerMapperExtension
{
    extension(Banner banner)
    {
        public BannerDto ToDto(string url)
        {
            return new BannerDto
            {
                CreatedAt = banner.CreatedAt,
                EndAt = banner.EndAt,
                Id = banner.Id,
                Image = string.IsNullOrEmpty(banner.Image) ? "" : url + banner.Image,
                StoreId = banner.StoreId,
            };
        }
    }
}