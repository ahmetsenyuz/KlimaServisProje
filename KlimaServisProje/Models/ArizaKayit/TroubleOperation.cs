using System.ComponentModel.DataAnnotations.Schema;

namespace KlimaServisProje.Models.ArizaKayit
{
    public class TroubleOperation
    {
        public int TroubleId { get; set; }

        [ForeignKey(nameof(TroubleId))]
        public  TroubleRegister TroubleRegister { get; set; }
        public int OperationId { get; set; }
        [ForeignKey(nameof(OperationId))]
        public OperationPrice OperationPrice { get; set; }
        public decimal Price { get; set; }
    }
}
