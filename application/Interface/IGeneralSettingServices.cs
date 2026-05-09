using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IGeneralSettingServices
{
   Task<GeneralSettingDto?> CreateGeneralSetting(Guid adminId, GeneralSettingDto settingDto);
   Task<GeneralSettingDto?> UpdateGeneralSetting(Guid id ,Guid adminId,UpdateGeneralSettingDto settingDto);
   
   Task<bool> DeleteGeneralSetting(Guid id,Guid adminId);
   
   Task<List<GeneralSettingDto>> GetGeneralSettings(int pageNum, int pageSize);
}