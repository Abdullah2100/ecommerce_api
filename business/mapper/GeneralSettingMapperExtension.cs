using api.domain.entity;
using data.dto.Request;
using data.dto.Response;

namespace business.mapper;

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