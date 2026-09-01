using api.application;
using api.domain.entity;
using data.Interface;
using data.util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace data.Repositories;

/// <summary>
/// Repository implementation for managing <see cref="ReseatPasswordOtp"/> entities.
/// Handles the generation, retrieval, validation, and cleanup of One-Time Passwords (OTPs) for password resets.
/// </summary>
/// <param name="context">The database context used for data access.</param>
/// <param name="logger">The logger used to log generated SQL queries.</param>
public class ReseatPasswordRepository(
    AppDbContext context,
    ILogger<ReseatPasswordRepository> logger
) : IRecreatePasswordRepository
{
    /// <summary>
    /// Retrieves a paged collection of all reset password OTP records.
    /// </summary>
    /// <param name="page">The page number to retrieve (1-indexed).</param>
    /// <param name="length">The number of items per page.</param>
    /// <returns>A task representing the asynchronous operation, returning a collection of OTP entities.</returns>
    public async Task<ICollection<ReseatPasswordOtp>> GetAllAsync(int page, int length)
    {
        var query = context
            .ReseatPasswords
            .AsNoTracking()
            .Skip((page - 1) * length)
            .Take(length);

        ClsUtil.logSql<ReseatPasswordRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.ToListAsync();
    }

    /// <summary>
    /// Adds a new password reset OTP to the database context.
    /// </summary>
    /// <param name="entity">The OTP entity to add.</param>
    public void Add(ReseatPasswordOtp entity)
    {
        context.ReseatPasswords.Add(entity);
    }

    /// <summary>
    /// Updates an existing password reset OTP in the database context.
    /// </summary>
    /// <param name="entity">The OTP entity with updated values.</param>
    public void Update(ReseatPasswordOtp entity)
    {
        context.ReseatPasswords.Update(entity);
    }

    /// <summary>
    /// Deletes all password reset OTPs associated with the email of the specified OTP identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the reference OTP.</param>
    /// <exception cref="ArgumentNullException">Thrown when the OTP with the specified ID is not found.</exception>
    public async Task Delete(Guid id)
    {
        var entity = await context.ReseatPasswords.FindAsync(id);
        if (entity == null) throw new ArgumentNullException();

        var query = context.ReseatPasswords
            .AsNoTracking()
            .Where(f => f.Email == entity.Email);

        ClsUtil.logSql<ReseatPasswordRepository>(
            logger,
            query.ToQueryString()
        );

        var previousPassword = await query.ToListAsync();
        if (previousPassword.Count == 0) return;
        context.ReseatPasswords.RemoveRange(previousPassword!);
    }

    /// <summary>
    /// Retrieves a password reset OTP by its string code.
    /// </summary>
    /// <param name="otp">The OTP code to search for.</param>
    /// <returns>A task representing the asynchronous operation, returning the OTP entity if found; otherwise, <c>null</c>.</returns>
    public async Task<ReseatPasswordOtp?> GetOtp(string otp)
    {
        var query = context.ReseatPasswords
            .AsNoTracking()
            .Where(f => f.Otp == otp);

        ClsUtil.logSql<ReseatPasswordRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Deletes all OTP records for a specific email, except for the current valid one.
    /// </summary>
    /// <param name="email">The email address to clean up.</param>
    /// <param name="otp">The current valid OTP code to exclude from deletion.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async Task DeleteAllEmailOtp(string email, string otp)
    {
        var query = context.ReseatPasswords.Where(rp => rp.Email == email && rp.Otp != otp);

        ClsUtil.logSql<ReseatPasswordRepository>(
            logger,
            query.ToQueryString()
        );

        await query.ExecuteDeleteAsync();
    }

    /// <summary>
    /// Validates if an OTP exists for a specific email and performs a cleanup of any previous OTPs for that email.
    /// </summary>
    /// <param name="otp">The OTP code to validate.</param>
    /// <param name="email">The email address associated with the OTP.</param>
    /// <returns>A task representing the asynchronous operation, returning <c>true</c> if the OTP exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> IsExist(string otp, string email)
    {
        // Delete previous OTPs for this email to ensure only the current one remains
        await DeleteAllEmailOtp(email, otp);

        var passwordOtp = await GetOtp(otp);
        return passwordOtp is not null;
    }

    /// <summary>
    /// Retrieves a password reset OTP by its code, email, and validation state, while also checking for expiration.
    /// </summary>
    /// <param name="otp">The OTP code.</param>
    /// <param name="email">The associated email address.</param>
    /// <param name="state">The validation state (<c>IsValidated</c>) to match.</param>
    /// <returns>A task representing the asynchronous operation, returning the OTP if it exists and has not expired; otherwise, <c>null</c>.</returns>
    /// <remarks>
    /// An OTP is considered expired if its <c>CreatedAt</c> timestamp is greater than the current time
    /// (Note: The current logic assumes <c>CreatedAt</c> represents the expiration time).
    /// </remarks>
    public async Task<ReseatPasswordOtp?> GetOtp(string otp, string email, bool state)
    {
        var query = context.ReseatPasswords
            .AsNoTracking()
            .Where(f => f.Otp == otp && f.Email == email && f.IsValidated == state);

        ClsUtil.logSql<ReseatPasswordRepository>(
            logger,
            query.ToQueryString()
        );

        var otpHolder = await query.FirstOrDefaultAsync();

        if (otpHolder is null) return null;

        // Check if the OTP is still valid based on the expiration timestamp
        if (otpHolder.CreatedAt > DateTime.Now)
        {
            return otpHolder;
        }

        return null;
    }
}