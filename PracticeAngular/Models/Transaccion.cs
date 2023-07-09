using System.ComponentModel.DataAnnotations;

namespace PracticeAngular.Models
{
    public class Transaccion
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }


        [Display(Name = "Fecha Transacción")]
        [DataType(DataType.Date)]
        public DateTime FechaTransaccion { get; set; } = DateTime.Now;

        [Required]
        public decimal Monto { get; set; }  

        public string Nota { get; set; }


        [Display(Name ="Tipos de Cuentas")]
        public int CuentaId { get; set; }


        [Display(Name ="Tipos de Categoria")]
        public int CategoriaId { get; set; }


        [Display(Name = "Tipo de Operación")]

        public TipoOperacionEnum TipoOperacionId { get; set; } = TipoOperacionEnum.Ingreso;
    }
}
