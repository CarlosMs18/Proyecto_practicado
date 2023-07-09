using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PracticeAngular.Models;
using PracticeAngular.Servicios;

namespace PracticeAngular.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IRepositorioUsuario repositorioUsuario;
        private readonly IRepositorioCuenta repositorioCuenta;
        private readonly IRepositorioCategoria repositorioCategoria;

        public TransaccionesController(
            IRepositorioUsuario repositorioUsuario,
            IRepositorioCuenta repositorioCuenta,
            IRepositorioCategoria repositorioCategoria
            )
        {
            this.repositorioUsuario = repositorioUsuario;
            this.repositorioCuenta = repositorioCuenta;
            this.repositorioCategoria = repositorioCategoria;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
           
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var modelo = new TransaccionViewModel();
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.Categorias = await ObtenerCategorias(usuarioId,modelo.TipoOperacionId);

            return View(modelo);
        }


        [HttpPost]
        public IActionResult Crear(int xd)
        {
            return View();
        }

        public async Task<IEnumerable<SelectListItem>> ObtenerCuentas(int usuarioId)
        {
            var cuentas = await repositorioCuenta.ObtenerCuentas(usuarioId);
            return cuentas.Select(x => new SelectListItem(x.Nombre , x.Id.ToString()));
        }

        private async Task<IEnumerable<SelectListItem>> ObtenerCategorias(int usuarioId, TipoOperacionEnum tipoOperacion)
        {
            var categorias = await repositorioCategoria.ObtenerCategorias(usuarioId , tipoOperacion);
            return categorias.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

        [HttpPost]
        public async Task<IActionResult> ObtenerCategorias([FromBody] TipoOperacionEnum tipoOperacion)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var categorias = await ObtenerCategorias(usuarioId, tipoOperacion);
            return Ok(categorias);
        }
    }
}
