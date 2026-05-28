using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.Presentation.dto.Response;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Implement;

public class GeneralSettingServices(IUnitOfWork unitOfWork) : IGeneralSettingServices
{
    public async Task<IActionResult> CreateGeneralSetting(Guid adminId, GeneralSettingDto settingDto)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (await unitOfWork.GeneralSettingRepository.IsExist(settingDto.Name))
        {
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
            return new ObjectResult("error while adding general setting")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var generalSettingToDto = generalSetting?.ToDto();

        return new ObjectResult(generalSettingToDto)
            { StatusCode = StatusCodes.Status201Created };
    }


    public async Task<IActionResult> UpdateGeneralSetting(
        Guid id, Guid adminId,
        UpdateGeneralSettingDto settingDto
    )
    {
        if (settingDto.IsEmpty())
            return new ObjectResult("no change found")
                { StatusCode = StatusCodes.Status200OK };


        var user = await unitOfWork.UserRepository
            .GetUser(adminId);

        var validationResult = user.IsValidateFunc();
        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var generalSetting = await unitOfWork.GeneralSettingRepository.GetGeneralSetting(id);

        if (generalSetting is null)
        {
            return new ObjectResult("no general setting found") { StatusCode = StatusCodes.Status404NotFound };
        }

        generalSetting.Name = settingDto.Name ?? generalSetting.Name;
        generalSetting.Value = settingDto.Value ?? generalSetting.Value;
        generalSetting.UpdatedAt = DateTime.Now;

        unitOfWork.GeneralSettingRepository.Add(generalSetting);
        var result = await unitOfWork.SaveChanges();


        if (result == 0)
        {
            return new ObjectResult("error while update general setting")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> DeleteGeneralSetting(Guid id, Guid adminId)
    {
        var user = await unitOfWork.UserRepository
            .GetUser(adminId);
        var validationResult = user.IsValidateFunc();

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        if (!(await unitOfWork.GeneralSettingRepository.IsExist(id)))
        {
            return new ObjectResult("generalSetting not found") { StatusCode = StatusCodes.Status404NotFound };
        }

        unitOfWork.GeneralSettingRepository.Delete(id);
        var result = await unitOfWork.SaveChanges();


        if (result == 0)
        {
            return new ObjectResult("error while delete general setting")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        return new ObjectResult(null)
            { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> GetGeneralSettings(int pageNum, int pageSize)
    {
        var categoriesToDto =
            (await unitOfWork.GeneralSettingRepository.Getgenralsettings(pageNum, pageSize))
            .Select(ca => ca.ToDto())
            .ToList();

        return new ObjectResult(categoriesToDto)
            { StatusCode = StatusCodes.Status200OK };
    }
    
    
}