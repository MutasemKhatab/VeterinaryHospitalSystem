/*using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Vet.Models
{
    public class Vaccine
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int VetId { get; set; }
        
        [ForeignKey("VetId")]
        public Vet Vet { get; set; }
        
        public ICollection<VaccineDose> Doses { get; set; } = new List<VaccineDose>();
    }

    public class VaccineDose
    {
        [Key]
        public int Id { get; set; }
        
        public int VaccineId { get; set; }
        
        [ForeignKey("VaccineId")]
        public Vaccine Vaccine { get; set; }
        
        [Required]
        public DateTime DoseDateTime { get; set; }
    }
}*/