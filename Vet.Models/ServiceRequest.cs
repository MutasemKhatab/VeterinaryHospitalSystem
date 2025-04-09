using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vet.Models;

public class ServiceRequest
{
    [Key] 
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    [Required]
    public DateTime RequestDate { get; set; } = DateTime.UtcNow;
    
    public DateTime? CompletionDate { get; set; }
    
    [Required]
    public string Status { get; set; } = "Pending"; // Pending, InProgress, Completed, Cancelled
    
    public string? VetOwnerId { get; set; }
    [JsonIgnore]
    public VetOwner VetOwner { get; set; }
}
