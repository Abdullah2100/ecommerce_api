using api.application;
using api.domain.entity;
using data.Interface;
using data.util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace data.Repositories;

/// <summary>
/// Provides data access operations for <see cref="UserRefreshToken"/> entities.
/// Supports creating, updating, and retrieving refresh tokens associated with users.
/// </summary>
/// <param name="context">The database context used to access user refresh token data.</param>
/// <param name="logger">The logger used to log generated SQL queries.</param>
public class UserRefreshTokenRepository(
    AppDbContext context,
    ILogger<UserRefreshTokenRepository> logger
) : IUserRefreshTokenRepository
{
    /// <summary>
    /// Determines whether a refresh token exists for the specified user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// is <c>true</c> if a refresh token exists for the user; otherwise, <c>false</c>.
    /// </returns>
    private async Task<bool> IsExistByUserId(Guid userId)
    {
        var query = context
            .UserRefreshTokens
            .AsNoTracking()
            .Where(value => value.UserId == userId);

        ClsUtil.logSql<UserRefreshTokenRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.AnyAsync();
    }

    /// <summary>
    /// Updates the refresh token and expiration time for an existing user token.
    /// </summary>
    /// <param name="data">
    /// The <see cref="UserRefreshToken"/> entity containing the updated
    /// refresh token and expiration information.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous update operation.
    /// </returns>
    private async Task UpdateUserRefreshToken(UserRefreshToken data)
    {
        var query = context.UserRefreshTokens
            .Where(user => user.UserId == data.UserId);

        ClsUtil.logSql<UserRefreshTokenRepository>(
            logger,
            query.ToQueryString()
        );

        await query.ExecuteUpdateAsync(value => value
            .SetProperty(value => value.ExpireAt, data.ExpireAt)
            .SetProperty(value => value.Refresh, data.Refresh));
    }

    /// <summary>
    /// Adds a new user refresh token to the database context.
    /// </summary>
    /// <param name="data">
    /// The <see cref="UserRefreshToken"/> entity to add.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous add operation.
    /// </returns>
    private async Task CreateUserRefreshToken(UserRefreshToken data)
    {
        await context.UserRefreshTokens.AddAsync(data);
    }

    /// <summary>
    /// Saves a user's refresh token by either creating a new record
    /// or updating the existing record associated with the user.
    /// </summary>
    /// <param name="data">
    /// The <see cref="UserRefreshToken"/> entity containing the user's
    /// refresh token and expiration information.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous save operation.
    /// </returns>
    public async Task Save(UserRefreshToken data)
    {
        var isExist = await IsExistByUserId(data.UserId);

        switch (isExist)
        {
            case true:
            {
                await UpdateUserRefreshToken(data);
            }
                break;

            default:
            {
                await CreateUserRefreshToken(data);
            }
                break;
        }
    }

    /// <summary>
    /// Retrieves the refresh token associated with the specified user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the user's refresh token if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<UserRefreshToken?> GetByUserId(Guid id)
    {
        var query = context
            .UserRefreshTokens
            .AsNoTracking()
            .Where(value => value.UserId == id);

        ClsUtil.logSql<UserRefreshTokenRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.FirstOrDefaultAsync();
    }
}