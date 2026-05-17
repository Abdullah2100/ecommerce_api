using api.domain.entity;

namespace api.domain.Interface;

public interface IUserRefreshTokenRepository
{
    public Task Save(UserRefreshToken data);
}