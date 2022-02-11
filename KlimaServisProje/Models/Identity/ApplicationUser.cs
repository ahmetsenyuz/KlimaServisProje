using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace KlimaServisProje.Models.Identity
{
    public class ApplicationUser : IdentityUser
    {
        [Required, StringLength(50)]
        [PersonalData]
        public string Name { get; set; }
        [Required, StringLength(50)]
        [PersonalData]
        public string Surname { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
