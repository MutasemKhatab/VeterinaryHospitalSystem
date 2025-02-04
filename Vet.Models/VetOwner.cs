using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Vet.Models {
    public class VetOwner : ApplicationUser {
        public List<Vet> Vets { get; init; } = [];
    }
}
