using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace KlimaServisProje.Models.ArizaKayit
{
    public class OperationPrice
    {
        [Key]
        public int operationId { get; set; }
        public string operationName { get; set; }
        public decimal price { get; set; }
        public string description { get; set; }
    }
}
