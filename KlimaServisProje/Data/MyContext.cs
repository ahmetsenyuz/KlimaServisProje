using KlimaServisProje.Models.ArizaKayit;
using KlimaServisProje.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KlimaServisProje.Data
{
    public class MyContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }
        public DbSet<OperationPrice> OperationPrices { get; set; }
        public DbSet<TroubleRegister> TroubleRegisters { get; set; }

    }
}
