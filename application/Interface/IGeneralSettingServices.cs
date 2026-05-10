using api.Presentation.dto;
using api.Presentation.dto.Request;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Interface;

public interface IGeneralSettingServices
{
    Task<IActionResult> CreateGeneralSetting(Guid adminId, GeneralSettingDto settingDto);
    Task<IActionResult> UpdateGeneralSetting(Guid id, Guid adminId, UpdateGeneralSettingDto settingDto);

    Task<IActionResult> DeleteGeneralSetting(Guid id, Guid adminId);

    Task<IActionResult> GetGeneralSettings(int pageNum, int pageSize);
}