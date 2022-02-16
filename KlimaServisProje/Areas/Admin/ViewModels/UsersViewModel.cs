using KlimaServisProje.Models.Identity;

namespace KlimaServisProje.Areas.Admin.ViewModels
{
    public class UsersViewModel
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public RoleViewModel Role { get; set; }
        public string roleName  { get; set; }
    }
}
