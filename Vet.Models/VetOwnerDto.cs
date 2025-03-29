namespace Vet.Models;

public class VetOwnerDto {
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? ProfilePicUrl { get; set; }

    public static VetOwnerDto FromVetOwner(VetOwner owner) {
        return new VetOwnerDto {
            Id = owner.Id,
            FirstName = owner.FirstName,
            LastName = owner.LastName,
            Email = owner.Email,
            PhoneNumber = owner.PhoneNumber,
            Address = owner.Address,
            ProfilePicUrl = owner.ProfilePicUrl
        };
    }
}