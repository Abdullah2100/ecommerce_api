using api.application;
using api.domain.entity;
using data.dto.Response;

namespace business.mapper;

public static class DeliveryMapperExtension
{
    extension(Delivery delivery)
    {
        public DeliveryDto ToDto(string url)
        {
            return new DeliveryDto
            {
                Id = delivery.Id,
                UserId = delivery.UserId,
                UpdatedAt = delivery.UpdatedAt,
                Address = delivery?.Address?.ToDeliveryDto(),
                Analyse = null,
                Thumbnail = string.IsNullOrEmpty(delivery?.Thumbnail) ? null : url + delivery.Thumbnail,
                User = delivery?.User.ToDeliveryInfoDto(url),
                IsAvailable = delivery?.IsAvailable ?? false
            };
        }
    }

    extension(Delivery? delivery)
    {
        public Tuple<string, int>? IsValidated()
        {
            if (delivery is null)
            {
                return new Tuple<string, int>("delivery not found", 404);
            }

            if (delivery.IsBlocked)
            {
                return new Tuple<string, int>("delivery is blocked", 403);
            }

            return null;
        }
    }
}