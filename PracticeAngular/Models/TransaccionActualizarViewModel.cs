using PracticeAngular.Models;

namespace PracticeAngular.Models
{
    public class TransaccionActualizarViewModel : TransaccionViewModel //heredamos de aca porque esta clase de transaccionACAUALIZARVIEWMODEL
                                                                               //lo que hara ser agregar las dos propiedades nuevas 

    {
        public int CuentaAnteriorId { get; set; }
        public decimal MontoAnterior { get; set; }
    }
}
