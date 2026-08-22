using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using business.mapper;
using api.util;
using data.dto.Request;
using data.dto.Response;
using data.util;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class GeneralSettingServices(IUnitOfWork unitOfWork, ILogger<GeneralSettingServices> logger) : IGeneralSettingServices
{
    public async Task<Result> CreateGeneralSetting(Guid adminId, GeneralSettingDto settingDto)
    {
        logger.LogInformation("start call create Genereal Setting");
        var user = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogInformation("user not valid {userId} validationError {message}", adminId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if (await unitOfWork.GeneralSettingRepository.IsExist(settingDto.Name))
        {
            logger.LogError("generalSetting name :{name} already exist", settingDto.Name);
            return new Result(false, "there are general setting with the same name", null, 409);
        }

        var generalSetting = new GeneralSetting
        {
            CreatedAt = DateTime.Now,
            Id = ClsUtil.GenerateGuid(),
            Name = settingDto.Name,
            Value = settingDto.Value
        };
        unitOfWork.GeneralSettingRepository.Add(generalSetting);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("coudl not saved generalSetting name:{name} to db", settingDto.Name);
            return new Result(false, "error while adding general setting", null, 500);
        }
        logger.LogInformation("create new general setting Name :{name} , adminId :{adminId}", generalSetting.Name, adminId);

        var generalSettingToDto = generalSetting?.ToDto();
        return new Result(true, null, generalSettingToDto, 201);
    }

    public async Task<Result> UpdateGeneralSetting(Guid id, Guid adminId, UpdateGeneralSettingDto settingDto)
    {
        logger.LogInformation("Start call Update generalsetting ");
        if (settingDto.IsEmpty())
        {
            logger.LogWarning("empty data sending to api");
            return new Result(false, "no change found", null, 200);
        }

        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var generalSetting = await unitOfWork.GeneralSettingRepository.GetGeneralSetting(id);

        if (generalSetting is null)
        {
            logger.LogError("general setting name :{name} with id :{id} not found", generalSetting.Name, generalSetting.Id);
            return new Result(false, "no general setting found", null, 404);
        }

        generalSetting.Name = settingDto.Name ?? generalSetting.Name;
        generalSetting.Value = settingDto.Value ?? generalSetting.Value;
        generalSetting.UpdatedAt = DateTime.Now;

        unitOfWork.GeneralSettingRepository.Add(generalSetting);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not update the name :{name} general setting to db", generalSetting.Name);
            return new Result(false, "error while update general setting", null, 500);
        }

        logger.LogInformation("complate update general setting ");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> DeleteGeneralSetting(Guid id, Guid adminId)
    {
        logger.LogInformation("start calling delete general setting");

        var user = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        if (!(await unitOfWork.GeneralSettingRepository.IsExist(id)))
        {
            logger.LogError("general setting  with id :{id} not found", id);
            return new Result(false, "generalSetting not found", null, 404);
        }

        unitOfWork.GeneralSettingRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            logger.LogError("could not delete  general setting with id {Id} from  db", id);
            return new Result(false, "error while delete general setting", null, 500);
        }

        logger.LogInformation("complate deleting generalSetting id:{Id}", id);
        return new Result(true, null, null, 204);
    }

    public async Task<Result> GetGeneralSettings(int pageNum, int pageSize)
    {
        logger.LogInformation("start getting generalSetting by page");
        var categoriesToDto = (await unitOfWork.GeneralSettingRepository.Getgenralsettings(pageNum, pageSize))
            .Select(ca => ca.ToDto())
            .ToList();
        logger.LogInformation("end getting generalSetting by page");
        return new Result(true, null, categoriesToDto, 200);
    }
}