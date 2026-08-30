using api.Request;
using data.dto.Request;
using data.Dto.Request;

namespace api.shared.mapper;

public static class CreateProductApiDtoMapper
{
    public static CreateProductDto ToBusinessDto(this CreateProductApiDto data)
    {
        using var thumbnailStream = new MemoryStream();
        data.Thumbnail.CopyTo(thumbnailStream);

        var images = data.Images?.Select(file =>
        {
            using var stream = new MemoryStream();
            file.CopyTo(stream);
            return stream.ToArray();
        }).ToList() ?? new List<byte[]>();

        return new CreateProductDto
        {
            Name = data.Name,
            Description = data.Description,
            Thumbnail = thumbnailStream.ToArray(),
            SubcategoryId = data.SubcategoryId,
            Price = data.Price,
            Symbol = data.Symbol,
            ProductVariants = data.ProductVariants,
            Images = images,
        };
    }
}
