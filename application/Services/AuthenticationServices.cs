using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.application.Interface;
using api.Infrastructure;
using api.util;
using Microsoft.IdentityModel.Tokens;

namespace api.application.Services
{
    public enum EnTokenMode
    {
        AccessToken,
        RefreshToken
    }


    public class AuthenticationServices(IConfig config) : IAuthenticationService
    {
        public string GenerateToken(Guid id, string email, EnTokenMode tokenType)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = config.GetKey("credentials:key");
            var issuer = config.GetKey("credentials:Issuer");
            var audience = config.GetKey("credentials:Audience");

            List<Claim> claims =
            [
                new(JwtRegisteredClaimNames.Jti, ClsUtil.GenerateGuid().ToString()),
                new(JwtRegisteredClaimNames.NameId, id.ToString() ?? ""),
                new(JwtRegisteredClaimNames.Email, email)
            ];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = ClsUtil.GenerateDateTime(tokenType),
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(key))
                    , SecurityAlgorithms.HmacSha256Signature)
            };


            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

     }
}