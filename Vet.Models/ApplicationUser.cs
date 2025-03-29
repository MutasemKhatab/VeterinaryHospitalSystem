using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Vet.Models;

public class ApplicationUser : IdentityUser {
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    public string? Address { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? ResetCode { get; set; }
}