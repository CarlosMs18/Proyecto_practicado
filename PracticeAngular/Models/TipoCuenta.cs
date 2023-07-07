using Microsoft.AspNetCore.Mvc;
using PracticeAngular.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace PracticeAngular.Models
{
    public class TipoCuenta
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:50 , MinimumLength = 3 ,ErrorMessage = "El campo {0} debe de estar en el rango de {2} - {1}")]
        [Display(Name = "Nombre del Tipo Cuenta")]
        [PrimerLetraMayuscula]
        [Remote(action: "ExisteTipoUsuario", controller: "TiposCuentas")]
        
        public string Nombre { get; set; }
        public int UsuarioId { get; set; }
        public int Orden { get; set; }  
    }
}
