using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Vet.Models;

public class Vet
{
    [Key] public int Id { get; set; }

    public string Name { get; set; }
    public string Gender { get; set; }
    public string Species { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? ProfilePicUrl { get; set; }
    public string? OwnerId { get; set; }
    [JsonIgnore]
    public VetOwner? Owner { get; set; }
}