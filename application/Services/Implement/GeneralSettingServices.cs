using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Implement;

public class GeneralSettingServices(IUnitOfWork unitOfWork, ILogger<GeneralSettingServices> logger) : IGeneralSettingServices
{
    public async Task<IActionResult> CreateGeneralSetting(Guid adminId, GeneralSettingDto settingDto)
    {
        logger.LogInformation("start call create Genereal Setting");
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogInformation("user not valid {userId} validationError {message}", adminId, validationResult.Item2);

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }


        if (await unitOfWork.GeneralSettingRepository.IsExist(settingDto.Name))
        {
            logger.LogError("generalSetting name :{name} already exist", settingDto.Name);
            return new ObjectResult("there are general setting with the same name")
            { StatusCode = StatusCodes.Status409Conflict };
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

            return new ObjectResult("error while adding general setting")
            { StatusCode = StatusCodes.Status500InternalServerError };
        }
        logger.LogInformation("create new general setting Name :{name} , adminId :{adminId}", generalSetting.Name, adminId);

        var generalSettingToDto = generalSetting?.ToDto();

        return new ObjectResult(generalSettingToDto)
        { StatusCode = StatusCodes.Status201Created };
    }


    public async Task<IActionResult> UpdateGeneralSetting(
        Guid id, Guid adminId,
        UpdateGeneralSettingDto settingDto
    )
    {
        logger.LogInformation("Start call Update generalsetting ");
        if (settingDto.IsEmpty())
        {
            logger.LogWarning("empty data sending to api");

            return new ObjectResult("no change found")
            { StatusCode = StatusCodes.Status200OK };
        }

        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validationResult.Item2);

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var generalSetting = await unitOfWork.GeneralSettingRepository.GetGeneralSetting(id);

        if (generalSetting is null)
        {
            logger.LogError("general setting name :{name} with id :{id} not found", generalSetting.Name, generalSetting.Id);

            return new ObjectResult("no general setting found") { StatusCode = StatusCodes.Status404NotFound };
        }

        generalSetting.Name = settingDto.Name ?? generalSetting.Name;
        generalSetting.Value = settingDto.Value ?? generalSetting.Value;
        generalSetting.UpdatedAt = DateTime.Now;

        unitOfWork.GeneralSettingRepository.Add(generalSetting);
        var result = await unitOfWork.SaveChanges();


        if (result == 0)
        {
            logger.LogError("could not update the name :{name} general setting to db", generalSetting.Name);

            return new ObjectResult("error while update general setting")
            { StatusCode = StatusCodes.Status500InternalServerError };
        }

        logger.LogInformation("complate update general setting ");

        return new ObjectResult(null)
        { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> DeleteGeneralSetting(Guid id, Guid adminId)
    {
        logger.LogInformation("start calling delete general setting");

        var user = await unitOfWork.UserRepository
            .GetUser(adminId);
        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validationResult.Item2);

            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (!(await unitOfWork.GeneralSettingRepository.IsExist(id)))
        {
            logger.LogError("general setting  with id :{id} not found", id);

            return new ObjectResult("generalSetting not found") { StatusCode = StatusCodes.Status404NotFound };
        }

        unitOfWork.GeneralSettingRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();


        if (result == 0)
        {
            logger.LogError("could not delete  general setting with id {Id} from  db", id);

            return new ObjectResult("error while delete general setting")
            { StatusCode = StatusCodes.Status500InternalServerError };
        }

        logger.LogInformation("complate deleting generalSetting id:{Id}", id);

        return new ObjectResult(null)
        { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> GetGeneralSettings(int pageNum, int pageSize)
    {
        logger.LogInformation("start getting generalSetting by page");
        var categoriesToDto =
            (await unitOfWork.GeneralSettingRepository.Getgenralsettings(pageNum, pageSize))
            .Select(ca => ca.ToDto())
            .ToList();
        logger.LogInformation("end getting generalSetting by page");

        return new ObjectResult(categoriesToDto)
        { StatusCode = StatusCodes.Status200OK };
    }
}