using Microsoft.AspNetCore.Mvc;
using api.application;
using data.dto.Request;
using data.dto.Response;

namespace api.application.Services.Interface;

public interface IGeneralSettingServices
{
    Task<Result> CreateGeneralSetting(Guid adminId, GeneralSettingDto settingDto);
    Task<Result> UpdateGeneralSetting(Guid id, Guid adminId, UpdateGeneralSettingDto settingDto);

    Task<Result> DeleteGeneralSetting(Guid id, Guid adminId);

    Task<Result> GetGeneralSettings(int pageNum, int pageSize);
}