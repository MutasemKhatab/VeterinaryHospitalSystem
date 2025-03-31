using System.ComponentModel.DataAnnotations;

namespace Vet.Models
{
    public class VaccineDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public int VetId { get; set; }

        public List<DateTime> Doses { get; set; } = new List<DateTime>();

        // Convert from DTO to entity
        public static Vaccine ToVaccine(VaccineDto dto)
        {
            return new Vaccine
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                VetId = dto.VetId,
                Doses = dto.Doses
            };
        }

        // Convert from entity to DTO
        public static VaccineDto FromVaccine(Vaccine vaccine)
        {
            return new VaccineDto
            {
                Id = vaccine.Id,
                Name = vaccine.Name,
                Description = vaccine.Description,
                VetId = vaccine.VetId,
                Doses = vaccine.Doses.ToList()
            };
        }
    }
}
