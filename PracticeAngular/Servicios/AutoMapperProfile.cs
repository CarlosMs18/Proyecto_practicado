using AutoMapper;

using PracticeAngular.Models;

namespace PracticeAngular.Servicios
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Cuenta, CuentaCreacionViewModel>();

            CreateMap<TransaccionActualizarViewModel, Transaccion>().ReverseMap(); //reverse map significa que vamos a llevar de transaccionAc a transacion y viceveersa, con el reversemap se hace esto en una solalinea
        }
    }
}
