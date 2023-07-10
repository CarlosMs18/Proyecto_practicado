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
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IServicioReportes servicioReportes;

        public CuentasController(
            IRepositorioCuenta repositorioCuenta,
            IRepositorioTiposCuentas repositorioTiposCuentas,
            IRepositorioUsuario repositorioUsuario,
            IMapper mapper,
            IRepositorioTransacciones repositorioTransacciones,
            IServicioReportes servicioReportes
            )
        {
            this.repositorioCuenta = repositorioCuenta;
            this.repositorioTiposCuentas = repositorioTiposCuentas;
            this.repositorioUsuario = repositorioUsuario;
            this.mapper = mapper;
            this.repositorioTransacciones = repositorioTransacciones;
            this.servicioReportes = servicioReportes;
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

        public async Task<IActionResult> Detalle(int id, int mes, int año)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var cuenta = await repositorioCuenta.ObtenerCuentaPorId(id, usuarioId);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            //DateTime fechaInicio;
            //DateTime fechaFin;

            //if(mes <= 0 || mes > 12 || año <= 1900)
            //{
            //    var hoy = DateTime.Today;
            //    fechaInicio = new DateTime(hoy.Year, hoy.Month, 1); //fecha de inicio sera el dia 1 del mes actual
            //}
            //else
            //{
            //    fechaInicio = new DateTime(año, mes, 1);
            //}

            //fechaFin = fechaInicio.AddMonths(1).AddDays(-1); //con esto estamos lelvandoa  dicha fin al ultimo dia del mismo mes de inicio

            //var obtenerTransaccionesPorCuenta = new ObtenerTransaccionesPorCuenta()
            //{
            //    CuentaId = id,
            //    UsuarioId = usuarioId,
            //    FechaInicio = fechaInicio,
            //    FechaFin = fechaFin
            //};

            //var transacciones = await repositorioTransacciones.ObtenerPorCuentaId(obtenerTransaccionesPorCuenta);

            //var modelo = new ReporteTransaccionesDetalladas();
            //ViewBag.Cuenta = cuenta.Nombre;

            //var transaccionesPorFecha = transacciones.OrderByDescending(x => x.FechaTransaccion) // quiero mostrar de manera ascendentw
            //                                                            .GroupBy(x => x.FechaTransaccion)
            //                                                            .Select(grupo => new ReporteTransaccionesDetalladas.TransaccionesPorFecha()
            //                                                            {
            //                                                                FechaTransaccion = grupo.Key,
            //                                                                Transacciones = grupo.AsEnumerable()
            //                                                            });

            //modelo.TransaccionesAgrupadas = transaccionesPorFecha;
            //modelo.FechaInicio = fechaInicio;
            //modelo.FechaFin = fechaFin;


            //ViewBag.mesAnterior = fechaInicio.AddMonths(-1).Month;  //si estamos enero esto nos botaria diciem bre
            //ViewBag.añoAnterior = fechaInicio.AddMonths(-1).Year;

            //ViewBag.mesPosterior = fechaInicio.AddMonths(1).Month;  //si estamos enero esto nos botaria diciem bre
            //ViewBag.añoPosterior = fechaInicio.AddMonths(1).Year;

            //ViewBag.urlRetorno = HttpContext.Request.Path + HttpContext.Request.QueryString;

            ViewBag.Cuenta = cuenta.Nombre;
            var modelo = servicioReportes.ObtenerReporteTransaccionesDetalladasPorCuenta(usuarioId,id, mes , año, ViewBag);
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
        public async Task<IActionResult> Borrar(int id  )
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
        public async Task<IActionResult> BorrarCuenta(int id , string urlRetorno = null)
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


  

