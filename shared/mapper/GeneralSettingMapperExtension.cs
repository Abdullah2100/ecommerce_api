using api.domain.entity;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.shared.mapper;

public static class GeneralSettingMapperExtension
{
    extension(GeneralSetting generalSetting)
    {
        public GeneralSettingDto ToDto()
        {
            return new GeneralSettingDto
            {
                Name = generalSetting.Name,
                Value = generalSetting.Value,
            };
        }
    }

    extension(UpdateGeneralSettingDto generalSetting)
    {
        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(generalSetting.Name?.Trim()) &&
                   generalSetting.Value != null;
        }
    }
}