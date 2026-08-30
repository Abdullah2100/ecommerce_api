using System.ComponentModel.DataAnnotations;
using api.Request;
using api.util;
using data.dto.Request;

namespace api.shared.mapper;

public static class CreateCategoryDtoMapper
{

    extension(CreateCategoryApiDto data)
    {
        public CreateCategoryDto ToDto()
        {

            return new CreateCategoryDto(data.Name, data.Image.ToBytes());

        } 
    }
}