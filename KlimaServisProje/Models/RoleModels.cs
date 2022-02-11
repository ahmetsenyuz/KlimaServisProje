using KlimaServisProje.Models.Identity;
using System.Collections.Generic;

namespace KlimaServisProje.Models
{
    public class RoleModels
    {
        public static string Admin = "Admin";
        public static string User = "User";
        public static string Technician = "Technician";
        public static string Operator = "Operator";

        public static ICollection<string> Roles => new List<string>() { Admin, User, Technician, Operator };
    }
}
