using api.domain.entity;
using data.dto.Request;
using data.dto.Response;

namespace business.mapper;

public static class CategoryMapperExtension
{
    extension(Category category)
    {
        public CategoryDto ToDto(string url)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Image = string.IsNullOrEmpty(category.Image) ? "" : url + category.Image,
                Name = category.Name
            };
        }
    }
    extension(UpdateCategoryDto category)
    {
        public bool IsEmpty()
        {
            return string.IsNullOrWhiteSpace(category.Name) &&
                   category.Image == null;
        }
    }
    
    
}