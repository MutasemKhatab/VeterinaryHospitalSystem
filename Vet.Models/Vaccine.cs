using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vet.Models {
    public class Vaccine {
        [Key] public int Id { get; set; }

        [Required] public string Name { get; set; }

        public string Description { get; set; }

        public int VetId { get; set; }

        [ForeignKey("VetId")] public Vet Vet { get; set; }

        public IList<DateTime> Doses { get; set; } = new List<DateTime>();
    }
}
