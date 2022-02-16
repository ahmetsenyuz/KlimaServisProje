using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KlimaServisProje.Models.Identity;

namespace KlimaServisProje.ViewModels
{
    public class TroubleRegisterViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string ACModel { get; set; }
        public string ACType { get; set; }
        public int Capacity { get; set; }
        public string GasType { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public bool FeeStatus { get; set; }
        public string TechnicianId { get; set; }
        public bool TechnicianStatus { get; set; }
    }
}
