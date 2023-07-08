using AutoMapper;
using PracticeAngular.Models;

namespace PracticeAngular.Servicios
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();
        }
    }
}
