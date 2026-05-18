using api.domain.entity;

namespace api.domain.Interface;

public interface IUserRefreshTokenRepository
{
    public Task Save(UserRefreshToken data);
    public Task<UserRefreshToken?> GetByUserId(Guid id);
}