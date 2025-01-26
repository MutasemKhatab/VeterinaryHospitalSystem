using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Vet.Models {
    public class VetOwner : IdentityUser {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        
        public string? Address { get; set; }
        [Url]
        public string? ProfilePicUrl { get; set; }

        public List<Vet> Vets { get; set; } = [];

    }
}
