using System.ComponentModel.DataAnnotations;

namespace KlimaServisProje.ViewModels
{
    public class OperationPriceViewModal
    {
        public int operationId { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Lütfen 50 karakterden kısa bir isim giriniz!")]
        public string operationName { get; set; }
        [Range(0, 50000, ErrorMessage = "Lütfen Geçerli Bir Sayı Giriniz!")]
        public decimal price { get; set; }
        [StringLength(450,ErrorMessage = "Lütfen 450 karakterden kısa bir açıklama giriniz!")]
        public string description { get; set; }
    }
}
