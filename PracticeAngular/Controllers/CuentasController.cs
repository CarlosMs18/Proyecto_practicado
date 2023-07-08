using AutoMapper;
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
        private readonly IMapper mapper;

        public CuentasController(
            IRepositorioCuenta repositorioCuenta,
            IRepositorioTiposCuentas repositorioTiposCuentas,
            IRepositorioUsuario repositorioUsuario,
            IMapper mapper
            )
        {
            this.repositorioCuenta = repositorioCuenta;
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.repositorioUsuario = repositorioUsuario;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var cuentasConTipoCuenta = await repositorioCuenta.ObtenerCuentas(usuarioId);
            var modelo = cuentasConTipoCuenta
                        .GroupBy(x => x.TipoCuenta)
                        .Select(grupo => new IndiceCuentaViewModel
                        {
                            TipoCuenta = grupo.Key,
                            Cuentas = grupo.AsEnumerable()
                        }).ToList();
            return View(modelo);
        }

        [HttpGet]
        public async Task<IActionResult> Crear()
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            

            var modelo = new CuentaCreacionViewModel();
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);



            return View(modelo);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(CuentaCreacionViewModel cuenta)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var tipoCuenta = await repositorioTiposCuentas.ObtenerTipoCuentaPorId(cuenta.TipoCuentaId, usuarioId);

            if (tipoCuenta is null)
            {
                return View("NoEncontrado", "Home");
            }

            if (!ModelState.IsValid)
            {
                var tiposCuentas = await repositorioTiposCuentas.ObtenerTiposCuentas(usuarioId);
                cuenta.TiposCuentas = tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
                return View(cuenta);
            }

            //var modelo = 
            await repositorioCuenta.CrearCuenta(cuenta);


            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var cuenta = await repositorioCuenta.ObtenerCuentaPorId(id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }



            return View(cuenta);
        }


        [HttpPost]
        public async Task<IActionResult> BorrarCuenta(int id)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var cuenta = await repositorioCuenta.ObtenerCuentaPorId(id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }
            await repositorioCuenta.EliminarCuenta(id);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var cuenta = await repositorioCuenta.ObtenerCuentaPorId(id, usuarioId);
            if (cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //var modelo = new CuentaCreacionViewModel()
            //{
            //    Id = cuenta.Id,
            //    Nombre = cuenta.Nombre,
            //    TipoCuentaId = cuenta.TipoCuentaId,
            //    Descripcion = cuenta.Descripcion,
            //    Balance = cuenta.Balance,
            //};


            var modelo = mapper.Map<CuentaCreacionViewModel>(cuenta);
            modelo.TiposCuentas = await ObtenerTiposCuentas(usuarioId);

            return View(modelo);
        }


        [HttpPost]
        public async Task<IActionResult> Editar(CuentaCreacionViewModel cuentaEditar)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var cuenta = repositorioCuenta.ObtenerCuentaPorId(cuentaEditar.Id, usuarioId);

            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            if(!ModelState.IsValid)
            {
                cuentaEditar.TiposCuentas = await ObtenerTiposCuentas(usuarioId);
                return View(cuentaEditar);
            }


            await repositorioCuenta.ActualizarCuenta(cuentaEditar);
            return RedirectToAction("Index");
        }


        public async Task<IEnumerable<SelectListItem>> ObtenerTiposCuentas(int usuarioId)
        {
            var tiposCuentas = await repositorioTiposCuentas.ObtenerTiposCuentas(usuarioId);
            return tiposCuentas.Select(x => new SelectListItem(x.Nombre, x.Id.ToString()));
        }

    }
}


  

