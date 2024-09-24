using Core.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Helpers
{
    public class JwtHelper
    {
        public static string GenerateToken(AppUser user, IConfiguration configuration)
        {
            var secretKey = configuration.GetValue<string>("JwtConfiguration:SecretKey");
            int accessTokenExpireHours = configuration.GetValue<int>("JwtConfiguration:AccessTokenExpireHours");

            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            }),
                Expires = DateTime.UtcNow.AddDays(accessTokenExpireHours),
                SigningCredentials =
                    new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
