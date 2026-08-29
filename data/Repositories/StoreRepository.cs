using api.application;
using api.domain.entity;
using data.Interface;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="UserRefreshToken"/> entities.
/// Handles the persistence and lifecycle of refresh tokens for user authentication.
/// </summary>
/// <param name="context">The database context used for data access.</param>
public class UserRefreshTokenRepository(AppDbContext context) : IUserRefreshTokenRepository
{
    /// <summary>
    /// Checks if a refresh token already exists for a specific user ID.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if the token exists; otherwise, <c>false</c>.</returns>
    private async Task<bool> IsExistByUserId(Guid userId)
    {
        return await context
            .UserRefreshTokens
            .AsNoTracking()
            .AnyAsync(value => value.UserId == userId);
    }

    /// <summary>
    /// Updates the refresh token value and expiration date for an existing user record.
    /// </summary>
    /// <param name="data">The refresh token entity containing updated values.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task UpdateUserRefreshToken(UserRefreshToken data)
    {
        await context.UserRefreshTokens
            .Where(user => user.UserId == data.UserId)
            .ExecuteUpdateAsync(value => value.SetProperty(value => value.ExpireAt, data.ExpireAt)
                .SetProperty(value => value.Refresh, data.Refresh));
    }

    /// <summary>
    /// Adds a new refresh token entity to the database context for tracking.
    /// </summary>
    /// <param name="data">The refresh token entity to create.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task CreateUserRefreshToken(UserRefreshToken data)
    {
        await context.UserRefreshTokens.AddAsync(data);
    }

    /// <summary>
    /// Saves a refresh token by either updating the existing one or creating a new record if it doesn't exist for the user.
    /// </summary>
    /// <param name="data">The refresh token data to save.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
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
    /// Retrieves the refresh token associated with a specific user ID.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>A task representing the asynchronous operation, returning the token if found; otherwise, <c>null</c>.</returns>
    public async Task<UserRefreshToken?> GetByUserId(Guid id)
    {
        return await context.UserRefreshTokens.AsNoTracking().FirstOrDefaultAsync(value => value.UserId == id);
    }
}