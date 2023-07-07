using System.ComponentModel.DataAnnotations;

namespace PracticeAngular.Models
{
    public class Cuenta
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "El {0} es requerido")]
        [StringLength(maximumLength:30)]
        [Display(Name = "Nombre Cuenta")] 
        public string Nombre { get; set; }

        [Display(Name = "Tipo de Cuenta")]
        public int TipoCuentaId { get; set; }

        public decimal Balance { get; set; }


      
        public string Descripcion { get; set; }
    }
}

