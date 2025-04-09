using System.ComponentModel.DataAnnotations;

namespace Vet.Models.DTOs;

public class ServiceRequestDto
{
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; }
    
    [Required]
    public string Description { get; set; }
    
    public DateTime RequestDate { get; set; }
    
    public DateTime? CompletionDate { get; set; }
    
    [Required]
    public string Status { get; set; }
    
    public string? VetOwnerId { get; set; }
    
    public ServiceRequest ToServiceRequest()
    {
        return new ServiceRequest
        {
            Id = Id,
            Title = Title,
            Description = Description,
            RequestDate = RequestDate,
            CompletionDate = CompletionDate,
            Status = Status,
            VetOwnerId = VetOwnerId
        };
    }
    
}
