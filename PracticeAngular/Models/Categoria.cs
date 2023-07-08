using System.ComponentModel.DataAnnotations;

namespace PracticeAngular.Models
{
    public class Categoria
    {

        public int Id { get; set; }


        [Required(ErrorMessage = "El {0} es requerido")]
        [StringLength(maximumLength: 30)]
        public string Nombre { get; set; }


        [Display(Name = "Tipo Operación")]
        public TipoOperacionEnum TipoOperacionId { get; set; }

        public int UsuarioId { get; set; }
    }
}
