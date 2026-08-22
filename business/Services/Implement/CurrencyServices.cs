using api.application;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using business.mapper;
using api.util;
using data.dto.Request;
using data.util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace business.Services.Implement;

public class CurrencyServices(
    IUnitOfWork unitOfWork,
    HybridCache cache,
    ILogger<CurrencyServices> logger) : ICurrencyServices
{
    public async Task<Result> CreateCurrency(Guid adminId, CreateCurrencyDto currencyDto)
    {
        logger.LogInformation("start create currency");
        var admin = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = admin.IsValidateFunc(true);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
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
            logger.LogError("could not saved currency to db");
            return new Result(false, "Could not Save New Currency", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.CurrenciesKey);
        var currencyToDto = currency.ToPaymentDto();

        logger.LogInformation("end create currency");
        return new Result(true, null, currencyToDto, 201);
    }

    public async Task<Result> UpdateCurrency(Guid adminId, UpdateCurrencyDto currencyDto)
    {
        logger.LogInformation("start update currency");
        var admin = await unitOfWork.UserRepository.GetUser(adminId);

        var validationResult = admin.IsValidateFunc(true);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var currency = await unitOfWork.CurrencyRepository.GetCurrencies(currencyDto.Id);

        if (currency is null)
        {
            logger.LogError("currency not found {currencyId}", currencyDto.Id);
            return new Result(false, "Currency is not found", null, 404);
        }

        currency.Name = currencyDto.Name ?? currency.Name;
        currency.Symbol = currencyDto.Symbol ?? currency.Symbol;
        currency.Value = currencyDto.Value ?? currency.Value;
        currency.UpdatedAt = DateTime.Now;

        unitOfWork.CurrencyRepository.Update(currency);
        var result = await unitOfWork.SaveChanges();
        if (result == 0)
        {
            logger.LogError("could not update currency in db");
            return new Result(false, "Could not Update Currency", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.CurrenciesKey);

        logger.LogInformation("end update currency");
        return new Result(true, null, null, 204);
    }

    public async Task<Result> DeleteCurrency(Guid adminId, Guid id)
    {
        logger.LogInformation("start delete currency");

        var admin = await unitOfWork.UserRepository.GetUser(adminId);
        var validationResult = admin.IsValidateFunc(true);

        if (validationResult is not null)
        {
            logger.LogError("user not valid {userId} validationError {message}", adminId, validationResult.Item2);
            return new Result(false, validationResult.Item1, null, validationResult.Item2);
        }

        var currency = await unitOfWork.CurrencyRepository.GetCurrencies(id);

        if (currency is null)
        {
            logger.LogError("currency not found with {currencyId}", id);
            return new Result(false, "currency not found", null, 404);
        }

        await unitOfWork.CurrencyRepository.Delete(currency.Id);
        var result = await unitOfWork.SaveChanges();
        if (result == 0)
        {
            logger.LogError("not able to delete {currencyId} from db", id);
            return new Result(false, "Could not Delete Currency", null, 500);
        }

        await cache.RemoveByTagAsync(MemoryCacheKeys.CurrenciesKey);

        logger.LogInformation("delete currency {currencyId}", id);
        return new Result(true, null, null, 204);
    }

    public async Task<Result> GetCurrency(int pageNum, int pageSize)
    {
        logger.LogInformation("start get currency by page ");

        var currencies = await cache.GetOrCreateAsync(MemoryCacheKeys.CurrenciesKey + pageNum, async ct =>
            {
                var currencies = (await unitOfWork.CurrencyRepository
                        .GetAll(pageNum, pageSize))
                    .Select(payment => payment.ToPaymentDto()).ToList();
                return currencies;
            },
            tags: [MemoryCacheKeys.CurrenciesKey]);

        logger.LogInformation("end get currency by page");
        return new Result(true, null, currencies, 200);
    }
}