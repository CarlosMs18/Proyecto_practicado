using Microsoft.AspNetCore.Mvc.Rendering;

namespace PracticeAngular.Models
{
    public class CuentaCreacionViewModel : Cuenta
    {

        public IEnumerable<SelectListItem> TiposCuentas { get; set; }
    }
}
