using api.application;
using api.domain.entity;
using data.Interface;
using data.util;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace data.Repositories;

/// <summary>
/// Provides data access operations for <see cref="User"/> entities.
/// Supports retrieving users by identifier, email, store, or credentials,
/// retrieving paginated users and address counts, checking user existence,
/// and performing basic entity management operations.
/// </summary>
/// <param name="dbContext">The database context used to access user and address data.</param>
/// <param name="logger">The logger used to log generated SQL queries.</param>
public class UserRepository(
    AppDbContext dbContext,
    ILogger<UserRepository> logger
) : IUserRepository
{
    /// <summary>
    /// Retrieves a user by their unique identifier.
    /// The user's associated store and addresses are also loaded.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the user if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<User?> GetUser(Guid id)
    {
        var query = dbContext
            .Users
            .Include(u => u.Store)
            .AsNoTracking()
            .Where(u => u.Id == id);

        ClsUtil.logSql<UserRepository>(
            logger,
            query.ToQueryString()
        );

        User? user = await query.FirstOrDefaultAsync();

        if (user == null) return null;

        var addressQuery = dbContext
            .Address
            .AsNoTracking()
            .Where(u => u.OwnerId == id);

        ClsUtil.logSql<UserRepository>(
            logger,
            addressQuery.ToQueryString()
        );

        user.Addresses = await addressQuery.ToListAsync();

        return user;
    }

    /// <summary>
    /// Retrieves a user by their email address.
    /// The user's associated store and addresses are also loaded.
    /// </summary>
    /// <param name="email">The email address of the user.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the user if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<User?> GetUser(string email)
    {
        var query = dbContext
            .Users
            .Include(u => u.Store)
            .AsNoTracking()
            .Where(u => u.Email == email);

        ClsUtil.logSql<UserRepository>(
            logger,
            query.ToQueryString()
        );

        User? user = await query.FirstOrDefaultAsync();

        if (user == null) return null;

        var addressQuery = dbContext
            .Address
            .AsNoTracking()
            .Where(u => u.OwnerId == user.Id);

        ClsUtil.logSql<UserRepository>(
            logger,
            addressQuery.ToQueryString()
        );

        user.Addresses = await addressQuery.ToListAsync();

        return user;
    }

    /// <summary>
    /// Gets the total number of users.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the total number of users.
    /// </returns>
    public async Task<int> GetUserCount()
    {
        var query = dbContext
            .Users
            .AsNoTracking();

        ClsUtil.logSql<UserRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.CountAsync();
    }

    /// <summary>
    /// Gets the total number of addresses associated with a user.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the number of addresses belonging to the specified user.
    /// </returns>
    public async Task<int> GetUserAddressCount(Guid id)
    {
        var query = dbContext
            .Address
            .AsNoTracking()
            .Where(u => u.OwnerId == id);

        ClsUtil.logSql<UserRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.CountAsync();
    }

    /// <summary>
    /// Retrieves the user associated with the specified store.
    /// The user's associated store and addresses are also loaded.
    /// </summary>
    /// <param name="id">The unique identifier of the store.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the associated user if found; otherwise, <c>null</c>.
    /// </returns>
    public async Task<User?> GetUserByStoreId(Guid id)
    {
        var query = dbContext
            .Users
            .Include(u => u.Store)
            .AsSplitQuery()
            .AsNoTracking()
            .Where(u => u.Store != null && u.Store.Id == id);

        ClsUtil.logSql<UserRepository>(
            logger,
            query.ToQueryString()
        );

        User? user = await query.FirstOrDefaultAsync();

        if (user == null) return null;

        var addressQuery = dbContext
            .Address
            .AsNoTracking()
            .Where(u => u.OwnerId == id);

        ClsUtil.logSql<UserRepository>(
            logger,
            addressQuery.ToQueryString()
        );

        user.Addresses = await addressQuery.ToListAsync();

        return user;
    }

    /// <summary>
    /// Retrieves a paginated collection of users.
    /// Each user's associated store and addresses are loaded.
    /// </summary>
    /// <param name="page">The one-based page number to retrieve.</param>
    /// <param name="length">The maximum number of users to return.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the requested page of users.
    /// </returns>
    public async Task<ICollection<User>> GetUsers(int page, int length)
    {
        var query = dbContext
            .Users
            .Include(u => u.Store)
            .AsSplitQuery()
            .AsNoTracking()
            .Skip((page - 1) * length)
            .OrderDescending()
            .Take(length);

        ClsUtil.logSql<UserRepository>(
            logger,
            query.ToQueryString()
        );

        ICollection<User>? users = await query.ToListAsync();

        foreach (var user in users)
        {
            var addressQuery = dbContext
                .Address
                .AsNoTracking()
                .Where(u => u.OwnerId == user.Id);

            ClsUtil.logSql<UserRepository>(
                logger,
                addressQuery.ToQueryString()
            );

            user.Addresses = await addressQuery.ToListAsync();
        }

        return users;
    }

    /// <summary>
    /// Retrieves a user using either their username or email address
    /// together with the specified password.
    /// The user's associated store and addresses are also loaded.
    /// </summary>
    /// <param name="username">
    /// The username or email address used to identify the user.
    /// </param>
    /// <param name="password">The password associated with the user account.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// contains the authenticated user if the credentials match;
    /// otherwise, <c>null</c>.
    /// </returns>
    public async Task<User?> GetUser(string username, string password)
    {
        try
        {
            var query = dbContext
                .Users
                .Include(u => u.Store)
                .AsNoTracking()
                .Where(u =>
                    (u.Name == username || u.Email == username) &&
                    u.Password == password);

            ClsUtil.logSql<UserRepository>(
                logger,
                query.ToQueryString()
            );

            User? user = await query.FirstOrDefaultAsync();

            if (user == null) return null;

            var addressQuery = dbContext
                .Address
                .AsNoTracking()
                .Where(u => u.OwnerId == user.Id);

            ClsUtil.logSql<UserRepository>(
                logger,
                addressQuery.ToQueryString()
            );

            user.Addresses = await addressQuery.ToListAsync();

            return user;
        }
        catch (System.Exception ex)
        {
            Console.WriteLine($"this the excptino error from get user {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Determines whether at least one user exists with the specified user role flag.
    /// </summary>
    /// <param name="role">
    /// The value of the <see cref="User.IsUser"/> flag to search for.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// is <c>true</c> if a matching user exists; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExist(bool role)
    {
        var query = dbContext
            .Users
            .AsNoTracking()
            .Where(u => u.IsUser == role);

        ClsUtil.logSql<UserRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.AnyAsync();
    }

    /// <summary>
    /// Determines whether a user exists with the specified phone number.
    /// </summary>
    /// <param name="phone">The phone number to check.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// is <c>true</c> if a user with the specified phone number exists;
    /// otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExistByPhone(string phone)
    {
        var query = dbContext
            .Users
            .AsNoTracking()
            .Where(u => u.Phone == phone);

        ClsUtil.logSql<UserRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.AnyAsync();
    }

    /// <summary>
    /// Determines whether a user exists with the specified email address.
    /// </summary>
    /// <param name="email">The email address to check.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// is <c>true</c> if a user with the specified email address exists;
    /// otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExistByEmail(string email)
    {
        var query = dbContext
            .Users
            .AsNoTracking()
            .Where(u => u.Email == email);

        ClsUtil.logSql<UserRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.AnyAsync();
    }

    /// <summary>
    /// Adds a user to the database context.
    /// The changes are not persisted until the context is saved.
    /// </summary>
    /// <param name="entity">The user entity to add.</param>
    public void Add(User entity)
    {
        dbContext.Users.Add(entity);
    }

    /// <summary>
    /// Marks a user as modified in the database context.
    /// The changes are not persisted until the context is saved.
    /// </summary>
    /// <param name="entity">The user entity containing the updated values.</param>
    public void Update(User entity)
    {
        dbContext.Users.Update(entity);
    }

    /// <summary>
    /// Soft-deletes a user by marking the account as blocked.
    /// The user record is not physically removed from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the user to block.</param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when a user with the specified identifier does not exist.
    /// </exception>
    public void Delete(Guid id)
    {
        User? user = dbContext.Users.Find(id);

        if (user == null)
            throw new ArgumentNullException();

        user.IsBlocked = true;
    }

    /// <summary>
    /// Determines whether a user exists with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the user.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result
    /// is <c>true</c> if the user exists; otherwise, <c>false</c>.
    /// </returns>
    public async Task<bool> IsExist(Guid id)
    {
        var query = dbContext.Users.Where(u => u.Id == id);

        ClsUtil.logSql<UserRepository>(
            logger,
            query.ToQueryString()
        );

        return await query.AnyAsync();
    }
}