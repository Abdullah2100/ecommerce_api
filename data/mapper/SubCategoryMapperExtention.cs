using api.domain.entity;
using data.dto.Request;
using data.dto.Response;

namespace data.mapper;

public static class SubCategoryMapperExtensions
{
    public static SubCategoryDto ToDto(this SubCategory? subCategory)
    {
        if (subCategory != null)
            return new SubCategoryDto
            {
                Id = subCategory.Id,
                Name = subCategory.Name,
                CategoryId = subCategory.CategoryId,
                StoreId = subCategory.StoreId
            };
        throw new ArgumentNullException(nameof(subCategory));

    }


    extension(UpdateSubCategoryDto dto)
    {
        public bool IsEmpty()
        {
            return dto == null
                ? throw new ArgumentNullException(nameof(dto))
                : string.IsNullOrWhiteSpace(dto.Name) &&
                  dto.CategoryId == null;
        }
    }
}