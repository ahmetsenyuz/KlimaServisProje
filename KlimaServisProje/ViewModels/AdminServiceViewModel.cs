namespace KlimaServisProje.ViewModels
{
    public class AdminServiceViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string AcInfo { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal TotalFee { get; set; }
        public bool FeeStatus { get; set; }
        public string TechName { get; set; }
        public bool Finished { get; set; }
    }
}
