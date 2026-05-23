using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.application.Services.Interface;
using api.domain.entity;
using api.Infrastructure;
using api.Presentation.dto.Response;
using api.Settings;
using api.util;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace api.application.Services.Implement
{
    public enum EnUserType
    {
        User,
        Admin,
        Delivery,
        Store
    }

    public class AuthenticationServices(IOptions<CredentialSetting> credential, IUnitOfWork unitOfWork)
        : IAuthenticationService
    {
        private async Task<Guid> GenerateRefreshToken(Guid userId, string role)
        {
            var userRefreshTokenHolder = new UserRefreshToken()
            {
                ExpireAt = DateTime.UtcNow.AddHours(4),
                UserId = userId,
                Id = Guid.CreateVersion7(),
                Refresh = Guid.CreateVersion7(),
                Role = role
            };
            await unitOfWork.UserRefreshTokenRepository.Save(userRefreshTokenHolder);
            await unitOfWork.SaveChanges();
            return userRefreshTokenHolder.Refresh;
        }

        public async Task<AuthDto> GenerateToken(Guid id, string email, EnUserType type)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = credential.Value.key;
            var issuer = credential.Value.Issuer;
            var audience = credential.Value.Audience;

            List<Claim> claims =
            [
                new(ClaimTypes.NameIdentifier, id.ToString() ?? ""),
                new(ClaimTypes.Email, email),
                new(ClaimTypes.Role, type.ToString())
            ];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(key))
                    , SecurityAlgorithms.HmacSha256Signature)
            };


            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenData = tokenHandler.WriteToken(token);
            var refreshToken = await GenerateRefreshToken(id, type.ToString());
            return new AuthDto() { Token = tokenData, RefreshToken = refreshToken };
        }
    }
}