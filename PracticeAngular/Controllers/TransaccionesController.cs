using AutoMapper;

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
        private readonly IRepositorioTransacciones repositorioTransacciones;
        private readonly IMapper mapper;

        public TransaccionesController(
            IRepositorioUsuario repositorioUsuario,
            IRepositorioCuenta repositorioCuenta,
            IRepositorioCategoria repositorioCategoria,
            IRepositorioTransacciones repositorioTransacciones,
            IMapper mapper
            )
        {
            this.repositorioUsuario = repositorioUsuario;
            this.repositorioCuenta = repositorioCuenta;
            this.repositorioCategoria = repositorioCategoria;
            this.repositorioTransacciones = repositorioTransacciones;
            this.mapper = mapper;
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
        public async Task<IActionResult> Editar(int id)
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
                return RedirectToAction("Index");
            }

        [HttpPost]
        public async Task<IActionResult> Borrar(int id)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var transaccion = await repositorioTransacciones.ObtenerPorId(id, usuarioId);  
            if(transaccion is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioTransacciones.Borrar(id);
            return RedirectToAction("Index");   
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
