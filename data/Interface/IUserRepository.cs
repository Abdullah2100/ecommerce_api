using api.domain.entity;
using data.dto.Response;

namespace data.Interface;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUser(Guid id);
    Task<User?> GetUser(string email, bool isTracking = true);
    Task<int> GetUserCount();
    Task<User?> GetUserByStoreId(Guid id);
    Task<ICollection<UserInfoDto>> GetUsers(int page, int length, string url);
    Task<User?> GetUser(string username, string password, bool isTracking = true);

    Task<bool> IsExist(Guid id);

    Task<bool> IsExist(bool role);
    Task<bool> IsExistByPhone(string phone);
    Task<bool> IsExistByEmail(string email);

    void Delete(Guid id);
}