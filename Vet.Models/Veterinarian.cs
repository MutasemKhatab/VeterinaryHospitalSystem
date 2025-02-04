using System.ComponentModel.DataAnnotations;

namespace Vet.Models;

public class Veterinarian : ApplicationUser
{
    [Required] public string EmployeeId { get; set; }
    [Required] public string Speciality { get; set; }
}