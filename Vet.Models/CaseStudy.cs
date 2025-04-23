using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Vet.Models;

public class CaseStudy {
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string Title { get; set; }
    [ForeignKey("VeterinarianId")] public string VeterinarianId { get; set; }
    [JsonIgnore] public Veterinarian? Veterinarian { get; set; }
    [ForeignKey("VetOwnerId")] public string VetOwnerId { get; set; }
    [JsonIgnore] public VetOwner? VetOwner { get; set; }
    [ForeignKey("VetId")] public int VetId { get; set; }
    [JsonIgnore] public Vet? Vet { get; set; }

    // History properties
    public string? Complaint { get; set; }
    public string? CaseHistory { get; set; }
    public string? PreviousTherapy { get; set; }

    // GeneralInspection properties
    public string? Git { get; set; }
    public string? RespiratoryTract { get; set; }
    public string? UrinaryTract { get; set; }
    public string? Skin { get; set; }
    public string? Cvs { get; set; }

    // GeneralSystemicConditions properties
    public string? Pulse { get; set; }
    public string? RespiratoryRate { get; set; }
    public string? Mm { get; set; }
    public string? Temperature { get; set; }
    public string? Demeanor { get; set; }
    public string? Ln { get; set; }

    // PhysicalExamination properties
    public string? ParanasalSinus { get; set; }
    public string? Lung { get; set; }
    public string? Heart { get; set; }
    public string? AbdomenLiver { get; set; }
    public string? CnsReflex { get; set; }

    // Diagnosis properties
    public string? DifferentialDiagnoses { get; set; }

    // Treatment properties
    public string? HygienicTtt { get; set; }
    public string? SpecificTtt { get; set; }
    public string? SupportiveTtt { get; set; }

    public string? PreventionControl { get; set; }
    public string? VaccinationProgram { get; set; }
    public string? DewormingProgram { get; set; }
    public string? AdditionalInfo { get; set; }
}
