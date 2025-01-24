using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vet.Models;

namespace VetApi.Utils
{
    public class TokenGenerator(IOptions<JwtSettings> jwtSettings)
    {
        public string GenerateToken(VetOwner vetOwner)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, vetOwner.Id),
                new Claim(JwtRegisteredClaimNames.Name, vetOwner.FirstName+" "+vetOwner.LastName),
                new Claim(JwtRegisteredClaimNames.Email, vetOwner.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: "http://localhost:8082",
                audience: "http://localhost:8082",
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}