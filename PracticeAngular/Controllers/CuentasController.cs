using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PracticeAngular.Models;
using PracticeAngular.Servicios;


namespace PracticeAngular.Controllers
{
    public class CuentasController : Controller
    {
        private readonly IRepositorioCuenta repositorioCuenta;
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IRepositorioUsuario repositorioUsuario;

        public CuentasController(
            IRepositorioCuenta repositorioCuenta,
            IRepositorioTiposCuentas repositorioTiposCuentas,
            IRepositorioUsuario repositorioUsuario
            )
        {
            this.repositorioCuenta = repositorioCuenta;
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.repositorioUsuario = repositorioUsuario;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var tiposCuentas = await repositorioTiposCuentas.ObtenerTiposCuentas(usuarioId);

            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
            
           
            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerTipoCuentaPorId(cuenta.TipoCuentaId, usuarioId);

            if(tipoCuenta is null)
            {
                return View("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                var tiposCuentas = await repositorioTiposCuentas.ObtenerTiposCuentas(usuarioId);
                cuenta.TiposCuentas = tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
                return View(tiposCuentas);
            }

            //var modelo = 
            await repositorioCuenta.CrearCuenta(cuenta);
            

            return RedirectToAction("Index");
        }

    }
}
