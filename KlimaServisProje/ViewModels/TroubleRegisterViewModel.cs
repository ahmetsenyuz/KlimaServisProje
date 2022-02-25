using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KlimaServisProje.Models.Identity;

namespace KlimaServisProje.ViewModels
{
    public class TroubleRegisterViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        [StringLength(50,ErrorMessage = "50 karakterden kısa giriniz.")]
        public string ACModel { get; set; }
        [StringLength(50, ErrorMessage = "50 karakterden kısa giriniz.")]
        public string ACType { get; set; }
        [Range(0, 50000, ErrorMessage = "Lütfen Geçerli Bir Sayı Giriniz!")]
        public int Capacity { get; set; } = 0;
        [StringLength(10, ErrorMessage = "10 karakterden kısa giriniz.")]
        public string GasType { get; set; }
        [StringLength(450, ErrorMessage = "450 karakterden kısa giriniz.")]
        public string Description { get; set; }
        [StringLength(100, ErrorMessage = "100 karakterden kısa giriniz.")]
        public string Address { get; set; }
        public bool FeeStatus { get; set; } = false;
        public string TechnicianId { get; set; }
        public bool TechnicianStatus { get; set; } = false;
        public bool Finished { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime StartedDate { get; set; }
        public DateTime FinishedDate { get; set; }
        public List<TroubleOperationViewModel> Operations { get; set; }
    }
}
