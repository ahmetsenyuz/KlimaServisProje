using AutoMapper;
using KlimaServisProje.Models.ArizaKayit;
using KlimaServisProje.ViewModels;

namespace KlimaServisProje.MapperProfile
{
    public class ReportProfile : Profile
    {
        public ReportProfile()
        {
            CreateMap<TroubleRegister, TroubleRegisterViewModel>().ReverseMap();
        }
    }
}
