using System.ComponentModel.DataAnnotations;

namespace KlimaServisProje.ViewModels
{
    public class TroubleOperationViewModel
    {
        public int TroubleId { get; set; }
        public int OperationId { get; set; }
        [Range(0, 10000, ErrorMessage = "Lütfen Geçerli Bir Sayı Giriniz!")]
        public decimal Price { get; set; }
    }
}
