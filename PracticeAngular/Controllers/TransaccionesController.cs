using AutoMapper;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PracticeAngular.Models;
using PracticeAngular.Servicios;
using System.Reflection;

namespace PracticeAngular.Controllers
{
    public class TransaccionesController : Controller
    {
        private readonly IRepositorioUsuario repositorioUsuario;
        private readonly IRepositorioCuenta repositorioCuenta;
        private readonly IRepositorioCategoria repositorioCategoria;
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IMapper mapper;
        private readonly IServicioReportes servicioReportes;

        public TransaccionesController(
            IRepositorioUsuario repositorioUsuario,
            IRepositorioCuenta repositorioCuenta,
            IRepositorioCategoria repositorioCategoria,
            IRepositorioTransacciones repositorioTransacciones,
            IMapper mapper,
            IServicioReportes servicioReportes
            )
        {
            this.repositorioUsuario = repositorioUsuario;
            this.repositorioCuenta = repositorioCuenta;
            this.repositorioCategoria = repositorioCategoria;
            this.repositorioTransacciones = repositorioTransacciones;
            this.mapper = mapper;
            this.servicioReportes = servicioReportes;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int mes , int año)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var modelo = await servicioReportes.ObtenerReporteTransaccionesDetalladas(usuarioId, mes, año, ViewBag);
            //DateTime fechaInicio;
            //DateTime fechaFin;

            //if (mes <= 0 || mes > 12 || año <= 1900)
            //{
            //    var hoy = DateTime.Today;
            //    fechaInicio = new DateTime(hoy.Year, hoy.Month, 1); //fecha de inicio sera el dia 1 del mes actual
            //}
            //else
            //{
            //    fechaInicio = new DateTime(año, mes, 1);
            //}

            //fechaFin = fechaInicio.AddMonths(1).AddDays(-1); //con esto estamos lelvandoa  dicha fin al ultimo dia del mismo mes de inicio

            //var parametro = new ParametroObtenerTransaccionesPorUsuario()
            //{
            //    UsuarioId = usuarioId,
            //    FechaInicio = fechaInicio,
            //    FechaFin = fechaFin,
            //};

            //var transacciones = await repositorioTransacciones.ObtenerPorUsuarioId(parametro);


            //var modelo = new ReporteTransaccionesDetalladas();
           

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
            return View(modelo);
        }

        public async Task<IActionResult> Semanal(int mes, int año)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            IEnumerable<ResultadoObtenerPorSemana> transaccionesPorSemana = 
                await servicioReportes.ObtenerReporteSemanal(usuarioId, mes, año, ViewBag);


            var agrupado = transaccionesPorSemana.GroupBy(x => x.Semana).Select(x => new ResultadoObtenerPorSemana()
            {
                Semana = x.Key,
                Ingresos = x.Where(x => x.TipoOperacionId == TipoOperacionEnum.Ingreso)
                                .Select(x => x.Monto).FirstOrDefault(),
                Gastos =  x.Where(x => x.TipoOperacionId == TipoOperacionEnum.Gasto)
                                .Select(x => x.Monto).FirstOrDefault(),

            }).ToList();

            if(año == 0 || mes == 0)
            {
                var hoy = DateTime.Today;
                año = hoy.Year;
                mes = hoy.Month;
            }

            var fechaReferencia = new DateTime(año, mes, 1);
            var diasDelMes = Enumerable.Range(1, fechaReferencia.AddMonths(1).AddDays(-1).Day);

            var diasSegmentados = diasDelMes.Chunk(7).ToList();

            for ( int i = 0; i< diasSegmentados.Count(); i++)
            {
                var semana = i + 1;
                var fechaInicio = new DateTime(año, mes, diasSegmentados[i].First());
                var fechaFin = new DateTime(año, mes, diasSegmentados[i].Last());
                var grupoSemana = agrupado.FirstOrDefault(x => x.Semana == semana); 
                if(grupoSemana is null)
                {
                    agrupado.Add(new ResultadoObtenerPorSemana()
                    {
                        Semana = semana,
                        FechaInicio = fechaInicio,
                        FechaFin = fechaFin
                    });
                }
                else
                {
                    grupoSemana.FechaInicio = fechaInicio;
                    grupoSemana.FechaFin = fechaFin;
                }
            }

            agrupado = agrupado.OrderByDescending(x => x.Semana).ToList();

            var modelo = new ReporteSemanaViewModel();
            modelo.TransaccionesPorSemana = agrupado;
            modelo.FechaReferencia = fechaReferencia;

            return View(modelo);
        }


        public async Task<IActionResult>  Mensual(int año)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            if(año == 0)
            {
                año = DateTime.Today.Year;
            }

            var transaccionPorMes = await repositorioTransacciones.ObtenerPorMes(usuarioId, año);
            var transaccionesAgrupadas = transaccionPorMes.GroupBy(x => x.Mes)
                                            .Select(x => new ResultadoObtenerPorMes()
                                            {
                                                Mes = x.Key,
                                                Ingreso = x.Where(x => x.TipoOperacionId == TipoOperacionEnum.Ingreso)
                                                                    .Select(x => x.Monto).FirstOrDefault(),
                                                Gasto = x.Where(x => x.TipoOperacionId == TipoOperacionEnum.Gasto)
                                                                    .Select(x => x.Monto).FirstOrDefault()
                                            }).ToList();
            
            for(int mes = 1; mes <=12; mes++)
            {
                var transaccion = transaccionesAgrupadas.FirstOrDefault(x => x.Mes == mes);
                var fechaReferencia = new DateTime(año, mes, 1);
                if(transaccion is null)
                {
                    transaccionesAgrupadas.Add(new ResultadoObtenerPorMes()
                    {
                        Mes = mes,
                        FechaReferencia = fechaReferencia
                    });

                }
                else
                {
                    transaccion.FechaReferencia = fechaReferencia;
                }
            }

            transaccionesAgrupadas = transaccionesAgrupadas.OrderByDescending( x => x.Mes).ToList();

            var modelo = new ReporteMensualViewModel();
            modelo.Año = año;
            modelo.TransaccionesPorMes = transaccionesAgrupadas;
            return View(modelo);
        }
        public IActionResult ReporteExcel()
        {
            return View();
        }
        public IActionResult Calendario()
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
        public  async Task<IActionResult> Crear(TransaccionViewModel modelo)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            if (!ModelState.IsValid)
            {
                modelo.Cuentas = await ObtenerCuentas(usuarioId);
                modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                return View(modelo);
            }

            var cuenta = await repositorioCuenta.ObtenerCuentaPorId(modelo.CuentaId, usuarioId);
            if(cuenta is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var categoria = await repositorioCategoria.ObtenerCategoriaPorId(modelo.CategoriaId, usuarioId);
            if(categoria is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            modelo.UsuarioId = usuarioId;
            if(modelo.TipoOperacionId == TipoOperacionEnum.Gasto)
            {
                modelo.Monto *= -1;
            }
            await repositorioTransacciones.Crear(modelo);

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id, string urlRetorno = null)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var transaccion =await  repositorioTransacciones.ObtenerPorId(id, usuarioId);
            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            var modelo = mapper.Map<TransaccionActualizarViewModel>(transaccion);

            modelo.MontoAnterior = modelo.Monto;
            if(modelo.TipoOperacionId == TipoOperacionEnum.Gasto)
            {
                modelo.MontoAnterior = modelo.Monto * -1;
            }

            modelo.CuentaAnteriorId = transaccion.CuentaId;
            modelo.Categorias = await ObtenerCategorias(usuarioId, transaccion.TipoOperacionId);
            modelo.Cuentas = await ObtenerCuentas(usuarioId);
            modelo.UrlRetorno = urlRetorno;
            return View(modelo);


        }

        [HttpPost]
        public async Task<IActionResult> Editar(TransaccionActualizarViewModel modelo)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
               
                if (!ModelState.IsValid)
                {
                    modelo.Cuentas = await ObtenerCuentas(usuarioId);
                    modelo.Categorias = await ObtenerCategorias(usuarioId, modelo.TipoOperacionId);
                    return View(modelo);
                }

                var cuenta = await repositorioCuenta.ObtenerCuentaPorId(modelo.CuentaId, usuarioId);
                if (cuenta is null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }

                var categoria = await repositorioCategoria.ObtenerCategoriaPorId(modelo.CategoriaId, usuarioId);
                if (categoria is null)
                {
                    return RedirectToAction("NoEncontrado", "Home");
                }

                var transaccion = mapper.Map<Transaccion>(modelo);
           

                if(modelo.TipoOperacionId == TipoOperacionEnum.Gasto)
                {
                    transaccion.Monto *= -1;
                }

            await repositorioTransacciones.Actualizar(transaccion, modelo.MontoAnterior, modelo.CuentaAnteriorId);
           

            if (string.IsNullOrEmpty(modelo.UrlRetorno))  //acceso a la transaccionactualiarViewModel ocn accseso a la url de retorno
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(modelo.UrlRetorno); //nos permite hacer na redireccion a una url que se encuentre dentro de nuestro dominio
               
            }

               
            }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id, string urlRetorno = null)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);  
            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTransacciones.Borrar(id);
            if (string.IsNullOrEmpty(urlRetorno))  //acceso a la transaccionactualiarViewModel ocn accseso a la url de retorno
            {
                return RedirectToAction("Index");
            }
            else
            {
                return LocalRedirect(urlRetorno); //nos permite hacer na redireccion a una url que se encuentre dentro de nuestro dominio

            }

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
