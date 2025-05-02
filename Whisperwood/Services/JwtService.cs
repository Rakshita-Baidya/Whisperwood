using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class JwtService
    {
        //obj of jwttoeknninfo class
        private readonly JwtTokenInfo _tokenInfo;

        public JwtService(IOptions<JwtTokenInfo> tokenInfoOption)
        {
            _tokenInfo = tokenInfoOption.Value;
        }

        public string GenerateToken(Users user)
        {
            string key = _tokenInfo.Key;

            //security key
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            //signin credentials obj
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, user.UserName!),
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            //obj that stores data to create a token
            var tokenObj = new JwtSecurityToken(
                    issuer: _tokenInfo.Issuer,
                    audience: _tokenInfo.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(_tokenInfo.ExpiryInMinutes),
                    signingCredentials: signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(tokenObj);
        }
    }
}