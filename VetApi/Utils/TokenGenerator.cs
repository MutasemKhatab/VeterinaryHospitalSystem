using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Vet.Models;

namespace VetApi.Utils {
    public class TokenGenerator(IOptions<JwtSettings> jwtSettings) {
        //TODO enhance this TokenGenerator
        public string GenerateToken(ApplicationUser user) {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.FirstName + " " + user.LastName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user is VetOwner ? "VetOwner" : "Veterinarian"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Value.Issuer,
                audience: jwtSettings.Value.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(900),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

/* TODO
To implement auto-renew tokens, you need to create an endpoint that will issue a new access token when the current one is about to expire. This typically involves using refresh tokens. Here is a step-by-step implementation:

   1. **Create a DTO for the token request**:
   ```csharp
   namespace Vet.Models
   {
       public class TokenRequestDto
       {
           public string AccessToken { get; set; }
           public string RefreshToken { get; set; }
       }
   }
   ```

   2. **Add a method to handle token renewal in your `AuthController`**:
   ```csharp
   using Microsoft.AspNetCore.Mvc;
   using Vet.DataAccess.Repository.IRepository;
   using Vet.Models;
   using Vet.Services;

   namespace VetApi.Controllers
   {
       [Route("api/[controller]")]
       [ApiController]
       public class AuthController : ControllerBase
       {
           private readonly IUnitOfWork _unitOfWork;
           private readonly ITokenService _tokenService;

           public AuthController(IUnitOfWork unitOfWork, ITokenService tokenService)
           {
               _unitOfWork = unitOfWork;
               _tokenService = tokenService;
           }

           [HttpPost("renew-token")]
           public async Task<IActionResult> RenewToken([FromBody] TokenRequestDto tokenRequest)
           {
               if (tokenRequest == null)
               {
                   return BadRequest("Invalid client request");
               }

               var principal = _tokenService.GetPrincipalFromExpiredToken(tokenRequest.AccessToken);
               if (principal == null)
               {
                   return BadRequest("Invalid access token or refresh token");
               }

               var userId = principal.Identity.Name;
               var user = await _unitOfWork.VetOwner.Get(u => u.Id == userId);
               if (user == null || user.RefreshToken != tokenRequest.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
               {
                   return BadRequest("Invalid access token or refresh token");
               }

               var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
               var newRefreshToken = _tokenService.GenerateRefreshToken();

               user.RefreshToken = newRefreshToken;
               await _unitOfWork.SaveAsync();

               return Ok(new
               {
                   AccessToken = newAccessToken,
                   RefreshToken = newRefreshToken
               });
           }
       }
   }
   ```

   3. **Implement the `ITokenService` interface**:
   ```csharp
   using System.Security.Claims;

   namespace Vet.Services
   {
       public interface ITokenService
       {
           string GenerateAccessToken(IEnumerable<Claim> claims);
           string GenerateRefreshToken();
           ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
       }
   }
   ```

   4. **Implement the `TokenService` class**:
   ```csharp
   using System;
   using System.Collections.Generic;
   using System.IdentityModel.Tokens.Jwt;
   using System.Linq;
   using System.Security.Claims;
   using System.Text;
   using Microsoft.Extensions.Configuration;
   using Microsoft.IdentityModel.Tokens;

   namespace Vet.Services
   {
       public class TokenService : ITokenService
       {
           private readonly IConfiguration _configuration;

           public TokenService(IConfiguration configuration)
           {
               _configuration = configuration;
           }

           public string GenerateAccessToken(IEnumerable<Claim> claims)
           {
               var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
               var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

               var tokenOptions = new JwtSecurityToken(
                   issuer: _configuration["Jwt:Issuer"],
                   audience: _configuration["Jwt:Audience"],
                   claims: claims,
                   expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:AccessTokenExpiryMinutes"])),
                   signingCredentials: signinCredentials
               );

               return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
           }

           public string GenerateRefreshToken()
           {
               var randomNumber = new byte[32];
               using (var rng = RandomNumberGenerator.Create())
               {
                   rng.GetBytes(randomNumber);
                   return Convert.ToBase64String(randomNumber);
               }
           }

           public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
           {
               var tokenValidationParameters = new TokenValidationParameters
               {
                   ValidateAudience = false,
                   ValidateIssuer = false,
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])),
                   ValidateLifetime = false // Here we are saying that we don't care about the token's expiration date
               };

               var tokenHandler = new JwtSecurityTokenHandler();
               var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

               if (!(securityToken is JwtSecurityToken jwtSecurityToken) || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
               {
                   throw new SecurityTokenException("Invalid token");
               }

               return principal;
           }
       }
   }
   ```

   This implementation includes:
   - A `TokenRequestDto` to handle token renewal requests.
   - An endpoint in `AuthController` to handle the token renewal logic.
   - A `TokenService` to generate and validate tokens.
   */