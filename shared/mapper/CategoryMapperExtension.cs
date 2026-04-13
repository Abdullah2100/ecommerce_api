using api.domain.entity;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.shared.mapper;

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