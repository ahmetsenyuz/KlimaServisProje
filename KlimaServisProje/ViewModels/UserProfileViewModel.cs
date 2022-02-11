using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KlimaServisProje.ViewModels
{
    public class UserProfileViewModel
    {
        [Required(ErrorMessage = "Isim Alanı Gereklidir.")]
        [Display(Name = "Ad")]
        [StringLength(50)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Soyad Alanı Gereklidir.")]
        [Display(Name = "Soyad")]
        [StringLength(50)]
        public string Surname { get; set; }
        [Required(ErrorMessage = "E-posta Alanı Gereklidir.")]
        [EmailAddress]
        public string Email { get; set; }
        public string UserName { get; internal set; }
    }
}
