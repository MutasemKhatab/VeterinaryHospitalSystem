using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Vet.Models;

public class Post {
    public int Id { get; set; }

    [Required] public string Title { get; set; }
    [Required] public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string? ImageUrl { get; set; }

    [ForeignKey("VeterinarianId")]
    [ValidateNever]
    public string VeterinarianId { get; set; }

/*if we can delete this mf field*/
    [ValidateNever] public Veterinarian Veterinarian { get; set; }
}