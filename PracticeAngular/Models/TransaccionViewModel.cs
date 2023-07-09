using Microsoft.AspNetCore.Mvc.Rendering;

namespace PracticeAngular.Models
{
    public class TransaccionViewModel : Transaccion
    {
        public IEnumerable<SelectListItem> Cuentas { get; set; }

        public IEnumerable<SelectListItem> Categorias {get; set; }


        //public TipoOperacionEnum TipoOperacionId { get; set; } = TipoOperacionEnum.Ingreso;
    }
}
