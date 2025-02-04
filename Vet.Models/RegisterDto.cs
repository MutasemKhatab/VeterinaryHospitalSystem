using System.ComponentModel.DataAnnotations;

namespace Vet.Models;

public class RegisterDto
{
    [Required] public string FirstName { get; set; }

    [Required] public string LastName { get; set; }

    [Required, EmailAddress] public string Email { get; set; }
    public string? Address { get; set; }
    public string? ProfilePicUrl { get; set; }
    [Required] public string Password { get; set; }

    // Expected values: "VetOwner" or "Veterinarian"
    [Required] public string UserType { get; set; }

    // Additional fields for Veterinarian
    public string? EmployeeId { get; set; }
    public string? Speciality { get; set; }

    public bool IsVetOwner => UserType.Equals("VetOwner", StringComparison.OrdinalIgnoreCase);
    public bool IsVeterinarian => UserType.Equals("Veterinarian", StringComparison.OrdinalIgnoreCase);
}