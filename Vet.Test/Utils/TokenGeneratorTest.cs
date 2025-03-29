using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Vet.Models;
using VetApi.Utils;

namespace Vet.Test.Utils;

[TestFixture]
[TestOf(typeof(TokenGenerator))]
public class TokenGeneratorTests {
    [SetUp]
    public void SetUp() {
        _jwtSettings = Options.Create(new JwtSettings {
            Key = "your_secret_key_here_long_long_long_one",
            Issuer = "http://localhost:8082",
            Audience = "http://localhost:8082"
        });
        _tokenGenerator = new TokenGenerator(_jwtSettings);
    }

    private TokenGenerator _tokenGenerator;
    private IOptions<JwtSettings> _jwtSettings;

    [Test]
    public void GenerateToken_ValidUser_ShouldGenerateToken() {
        // Arrange
        var user = new ApplicationUser {
            Id = "1",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var token = _tokenGenerator.GenerateToken(user);

        // Assert
        Assert.That(token, Is.Not.Null);
        Assert.That(token, Is.Not.Empty);
    }

    [Test]
    public void GenerateToken_ShouldContainCorrectClaims() {
        // Arrange
        var user = new ApplicationUser {
            Id = "1",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var token = _tokenGenerator.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        Assert.That(jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Sub).Value, Is.EqualTo(user.Id));
        Assert.That(jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Name).Value, Is.EqualTo($"{user.FirstName} {user.LastName}"));
        Assert.That(jwtToken.Claims.First(c => c.Type == JwtRegisteredClaimNames.Email).Value, Is.EqualTo(user.Email));
    }

    [Test]
    public void GenerateToken_VetOwner_ShouldContainRoleClaim() {
        // Arrange
        var user = new VetOwner {
            Id = "1",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var token = _tokenGenerator.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        Assert.That(jwtToken.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "VetOwner"), Is.True);
    }

    [Test]
    public void GenerateToken_Veterinarian_ShouldContainRoleClaim() {
        // Arrange
        var user = new ApplicationUser {
            Id = "1",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var token = _tokenGenerator.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Assert
        Assert.That(jwtToken.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "Veterinarian"), Is.True);
    }

    [Test]
    public void GenerateToken_InvalidKey_ShouldThrowException() {
        // Arrange
        var invalidJwtSettings = Options.Create(new JwtSettings {
            Key = "",
            Issuer = "your_issuer",
            Audience = "your_audience"
        });
        var invalidTokenGenerator = new TokenGenerator(invalidJwtSettings);
        var user = new ApplicationUser {
            Id = "1",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => invalidTokenGenerator.GenerateToken(user));
    }
    
    
}