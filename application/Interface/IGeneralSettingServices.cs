using api.application.Result;
using api.Presentation.dto;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;

namespace api.application.Interface;

public interface IGeneralSettingServices
{
   Task<Result<GeneralSettingDto?>> CreateGeneralSetting(Guid adminId, GeneralSettingDto settingDto);
   Task<Result<GeneralSettingDto?>> UpdateGeneralSetting(Guid id ,Guid adminId,UpdateGeneralSettingDto settingDto);
   
   Task<Result<bool>> DeleteGeneralSetting(Guid id,Guid adminId);
   
   Task<Result<List<GeneralSettingDto>>> GetGeneralSettings(int pageNum, int pageSize);
}