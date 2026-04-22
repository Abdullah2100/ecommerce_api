using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using api.application.Interface;
using api.Infrastructure;
using api.util;
using Microsoft.IdentityModel.Tokens;

namespace api.application
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
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            string key = config.GetKey("credentials:key");
            string issuer = config.GetKey("credentials:Issuer");
            string audience = config.GetKey("credentials:Audience");

            List<Claim> claims = new List<Claim>()
            {
                new(JwtRegisteredClaimNames.Jti, ClsUtil.GenerateGuid().ToString()),
                new(JwtRegisteredClaimNames.NameId, id.ToString() ?? ""),
                new(JwtRegisteredClaimNames.Email, email)
            };

            SecurityTokenDescriptor tokenDescip = new SecurityTokenDescriptor
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


            SecurityToken? token = tokenHandler.CreateToken(tokenDescip);
            return tokenHandler.WriteToken(token);
        }

       
    }
}