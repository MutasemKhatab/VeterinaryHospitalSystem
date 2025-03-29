using System.ComponentModel.DataAnnotations;
using Vet.Models;

namespace Vet.Test.Models;

[TestFixture]
[TestOf(typeof(RegisterDto))]
public class RegisterDtoTest {
    [Test]
    public void RegisterDto_ValidData_ShouldBeValid() {
        // Arrange
        var dto = new RegisterDto {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            Password = "Password123",
            UserType = "VetOwner"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Empty);
    }

    [Test]
    public void RegisterDto_MissingRequiredFields_ShouldBeInvalid() {
        // Arrange
        var dto = new RegisterDto();

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults, Has.Count.EqualTo(5)); // FirstName, LastName, Email, Password, and UserType are required
    }

    [Test]
    public void RegisterDto_InvalidEmail_ShouldBeInvalid() {
        // Arrange
        var dto = new RegisterDto {
            FirstName = "John",
            LastName = "Doe",
            Email = "invalid-email",
            Password = "Password123",
            UserType = "VetOwner"
        };

        // Act
        var validationResults = ValidateModel(dto);

        // Assert
        Assert.That(validationResults, Is.Not.Empty);
        Assert.That(validationResults.Any(v => v.MemberNames.Contains("Email")), Is.True);
    }

    [Test]
    public void RegisterDto_UserType_ShouldIdentifyVetOwner() {
        // Arrange
        var dto = new RegisterDto {
            UserType = "VetOwner"
        };

        // Act & Assert
        Assert.That(dto.IsVetOwner, Is.True);
        Assert.That(dto.IsVeterinarian, Is.False);
    }

    [Test]
    public void RegisterDto_UserType_ShouldIdentifyVeterinarian()
    {
        // Arrange
        var dto = new RegisterDto {
            UserType = "Veterinarian"
        };
        
        Assert.Multiple(() =>
        {
            // Act & Assert
            Assert.That(dto.IsVeterinarian, Is.True);
            Assert.That(dto.IsVetOwner, Is.False);
        });
    }

    private static List<ValidationResult> ValidateModel(object model) {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}