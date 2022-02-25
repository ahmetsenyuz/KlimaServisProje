using System;
using System.Collections.Generic;
using KlimaServisProje.ViewModels;

namespace KlimaServisProje.Areas.Admin.ViewModels
{
    public class TechnicianRecordsViewModel
    {
        public int Id { get; set; }
        public string UsersName { get; set; }
        public string ACModel { get; set; }
        public string ACType { get; set; }
        public int Capacity { get; set; }
        public string GasType { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public DateTime StartedDate { get; set; }
        public DateTime FinishDate  { get; set; }
        public bool Finished { get; set; }
    }
}
