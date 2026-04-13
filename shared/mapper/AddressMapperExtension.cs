using api.domain.entity;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.shared.mapper;

public static class AddressMapperExtension
{
    extension(Address address)
    {
        public AddressDto ToDto()
        {
            return new AddressDto
            {
                Id = address.Id,
                Latitude = address.Latitude,
                Longitude = address.Longitude,
                Title = address.Title,
                IsCurrent = address.IsCurrent
            };
        }

        public DeliveryAddressDto ToDeliveryDto()
        {
            return new DeliveryAddressDto
            {
                Latitude = address.Latitude,
                Longitude = address.Longitude,
            };
        }
    }

    extension(UpdateAddressDto dto)
    {
        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(dto.Title)
                   && dto.Longitude == null
                   && dto.Latitude == null;
        }
    }
}