using api.application;
using api.domain.entity;
using api.domain.Interface;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Repositories;

public class UserRefreshTokenRepository(AppDbContext context):IUserRefreshTokenRepository
{

    private async Task<bool> IsExistByUserId(Guid userId)
    {
        return await context
            .UserRefreshTokens
            .AsNoTracking()
            .AnyAsync(value=>value.UserId==userId);
    }


    private async Task UpdateUserRefreshToken(UserRefreshToken data)
    {
        await context.UserRefreshTokens
            .Where(user=>user.UserId==data.UserId)
            .ExecuteUpdateAsync(value=>value.SetProperty(value=>value.ExpireAt,data.ExpireAt)
                .SetProperty(value=>value.Refresh,data.Refresh));
    }
    
    private async Task CreateUserRefreshToken(UserRefreshToken data)
    {
        await context.UserRefreshTokens.AddAsync(data);
    }
    
    public async Task Save(UserRefreshToken data)
    {
        var isExist =await IsExistByUserId(data.UserId);
        switch (isExist)
        {
            case true:
            {
              await  UpdateUserRefreshToken(data);
            }
                break;
            default:
            {
                await CreateUserRefreshToken(data);
            }
                break;
        }
    }
}