using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Request;
using api.shared.mapper;
using api.util;
using Microsoft.AspNetCore.Mvc;

namespace api.application.Services.Implement;

public class CurrencyServices(IUnitOfWork unitOfWork) : ICurrencyServices
{
    public async Task<IActionResult> CreateCurrency(Guid adminId, CreateCurrencyDto currencyDto)
    {
        var admin = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = admin.IsValidateFunc(true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }



        var currency = new Currency
        {
            Id = ClsUtil.GenerateGuid(),
            CreatedAt = DateTime.Now,
            Symbol = currencyDto.Symbol,
            Name = currencyDto.Name,
            Value = currencyDto.Value,
            IsDefault = currencyDto.IsDefault
        };
        unitOfWork.CurrencyRepository.Add(currency);
        var result = await unitOfWork.SaveChanges();

        if (result == 0)
        {
            return new ObjectResult("Could not Save New Currency")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        var currencyToDto = currency.ToPaymentDto();
        return new ObjectResult(currencyToDto) { StatusCode = StatusCodes.Status201Created };
    }

    public async Task<IActionResult> UpdateCurrency(Guid adminId, UpdateCurrencyDto currencyDto)
    {
        var admin = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = admin.IsValidateFunc(true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var currency = await unitOfWork.CurrencyRepository.GetCurrencies(currencyDto.Id);

        if (currency is null)
        {
            return new ObjectResult("Currency is not found") { StatusCode = StatusCodes.Status404NotFound };
        }


        currency.Name = currencyDto.Name ?? currency.Name;
        currency.Symbol = currencyDto.Symbol ?? currency.Symbol;
        currency.Value = currencyDto.Value ?? currency.Value;
        currency.UpdatedAt = DateTime.Now;

        unitOfWork.CurrencyRepository.Update(currency);
        var result = await unitOfWork.SaveChanges();
        if (result == 0)
        {
            return new ObjectResult("Could not Update Currency")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }

        return new ObjectResult(null) { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> DeleteCurrency(Guid adminId, Guid id)
    {
        var admin = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = admin.IsValidateFunc(true);

        if (validationResult is not null)
        {
            return new ObjectResult(validationResult.Item1) { StatusCode = validationResult.Item2 };
        }

        var currency = await unitOfWork.CurrencyRepository.GetCurrencies(id);

        if (currency is null)
        {
            return new ObjectResult("currency not found") { StatusCode = StatusCodes.Status404NotFound };
        }


        await unitOfWork.CurrencyRepository.Delete(currency.Id);
        var result = await unitOfWork.SaveChanges();
        if (result == 0)
        {
            return new ObjectResult("Could not Delete Currency")
                { StatusCode = StatusCodes.Status500InternalServerError };
        }


        return new ObjectResult(null) { StatusCode = StatusCodes.Status204NoContent };
    }

    public async Task<IActionResult> GetCurrency(int pageNum, int pageSize)
    {
        var payments = await unitOfWork.CurrencyRepository.GetAll(pageNum, pageSize);

        var paymentToDto = payments.Select(payment => payment.ToPaymentDto()).ToList();
        return new ObjectResult(paymentToDto)
            { StatusCode = StatusCodes.Status200OK };

    }
}