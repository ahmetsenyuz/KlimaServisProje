using KlimaServisProje.Models.ArizaKayit;
using KlimaServisProje.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KlimaServisProje.Data
{
    public class MyContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public MyContext(DbContextOptions<MyContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)//decimal değerleri sqlde bozulmasın diye ayarlayacağız. öteki türlü virgülden sonra 18 hane alıyordu.
        {
            base.OnModelCreating(builder);
            builder.Entity<TroubleOperation>().HasKey(x => new { x.TroubleId, x.OperationId });
        }
        public DbSet<OperationPrice> OperationPrices { get; set; }
        public DbSet<TroubleRegister> TroubleRegisters { get; set; }
        public DbSet<TechniciansStatu> TechniciansStatus { get; set; }
        public DbSet<TroubleOperation> TroubleOperations { get; set; }
    }
}
