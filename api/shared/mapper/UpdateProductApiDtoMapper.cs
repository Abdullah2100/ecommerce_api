using api.Request;
using data.dto.Request;
using data.Dto.Request;

namespace api.shared.mapper;

public static class UpdateProductApiDtoMapper
{
    public static UpdateProductDto ToBusinessDto(this UpdateProductApiDto data)
    {
        using var thumbnailStream = new MemoryStream();
        if (data.Thumbnail is not null)
            data.Thumbnail.CopyTo(thumbnailStream);

        var images = data.Images?.Select(file =>
        {
            using var stream = new MemoryStream();
            file.CopyTo(stream);
            return stream.ToArray();
        }).ToList();

        return new UpdateProductDto
        {
            Id = data.Id,
            Name = data.Name,
            Description = data.Description,
            Thumbnail = data.Thumbnail is null ? null : thumbnailStream.ToArray(),
            SubcategoryId = data.SubcategoryId,
            StoreId = data.StoreId,
            Price = data.Price,
            Symbol = data.Symbol,
            ProductVariants = data.ProductVariants,
            DeletedProductVariants = data.DeletedProductVariants,
            Images = images,
            Deletedimages = data.Deletedimages,
        };
    }
}
