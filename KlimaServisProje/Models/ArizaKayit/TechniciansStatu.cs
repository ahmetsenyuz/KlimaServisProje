using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KlimaServisProje.Models.Identity;

namespace KlimaServisProje.Models.ArizaKayit
{
    public class TechniciansStatu
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Id")]
        public string TechnicianId { get; set; }
        public virtual ApplicationUser Technician { get; set; }
        public string Name { get; set; }
        public bool Status { get; set; } = false;
    }
}
