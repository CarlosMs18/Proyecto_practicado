using Microsoft.AspNetCore.Mvc;
using PracticeAngular.Models;
using PracticeAngular.Servicios;

namespace PracticeAngular.Controllers
{
    public class TiposCuentasController : Controller
    {
        private readonly IRepositorioTiposCuentas repositorioTiposCuentas;
        private readonly IRepositorioUsuario repositorioUsuario;

        public TiposCuentasController(
            IRepositorioTiposCuentas repositorioTiposCuentas,
            IRepositorioUsuario repositorioUsuario
            )
        {
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.repositorioUsuario = repositorioUsuario;
        }

        public async Task<IActionResult> Index()
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var tiposCuentas = await repositorioTiposCuentas.ObtenerTiposCuentas(usuarioId);


            return View(tiposCuentas);  
        }

       
        public IActionResult Crear()
        {
            

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Crear(TipoCuenta tipoCuentas)
        {
            if(!ModelState.IsValid)
            {
                return View(tipoCuentas);
            }

            tipoCuentas.UsuarioId = repositorioUsuario.obtenerUsuario();

            var existeTipoCuenta = await repositorioTiposCuentas.ExisteTipoCuenta(tipoCuentas.Nombre, tipoCuentas.UsuarioId);
            if( existeTipoCuenta)
            {
                ModelState.AddModelError(nameof(tipoCuentas.Nombre), $"El nombre {tipoCuentas.Nombre} ya existe en la base de datos!");
                return View(tipoCuentas);
            }

            await repositorioTiposCuentas.Crear(tipoCuentas);
           
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> ExisteTipoUsuario(string nombre)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var existeTipoCuenta = await repositorioTiposCuentas.ExisteTipoCuenta(nombre, usuarioId);
            if (existeTipoCuenta)
            {
                return Json($"El nombre {nombre} ya existe en la base de datos!");
            }
            return Json(true);

        }
    }
}
