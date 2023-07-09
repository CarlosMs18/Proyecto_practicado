using Microsoft.AspNetCore.Mvc;
using PracticeAngular.Models;
using PracticeAngular.Servicios;

namespace PracticeAngular.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly IRepositorioUsuario repositorioUsuario;
        private readonly IRepositorioCategoria repositorioCategoria;

        public CategoriasController(
            IRepositorioUsuario repositorioUsuario,
            IRepositorioCategoria repositorioCategoria
            )
        {
            this.repositorioUsuario = repositorioUsuario;
            this.repositorioCategoria = repositorioCategoria;
        }

        [HttpGet]
        public async Task<ActionResult> Index() {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var categorias = await repositorioCategoria.ObtenerCategorias(usuarioId);

            if(categorias is null)
            {
                return RedirectToAction("NoEncontrado", "Index");
            }


            return View(categorias);
        }

        [HttpGet]
        public ActionResult Crear()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Crear(Categoria categoria)
        {
            if (!ModelState.IsValid)
            {
                return View(categoria);
            }

            var usuarioId = repositorioUsuario.obtenerUsuario();
            categoria.UsuarioId = usuarioId;
            await repositorioCategoria.CrearCategoria(categoria);

            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var categoriaExiste = await repositorioCategoria.ObtenerCategoriaPorId( id , usuarioId);
            if(categoriaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoriaExiste);
        }

        [HttpPost]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var categoriaExiste = await repositorioCategoria.ObtenerCategoriaPorId(id, usuarioId);
            if (categoriaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCategoria.EliminarCategoria(id, usuarioId);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var categoriaExiste = await repositorioCategoria.ObtenerCategoriaPorId(id, usuarioId);
            if (categoriaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            return View(categoriaExiste);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(Categoria categoria)
        {
            var usuarioId = repositorioUsuario.obtenerUsuario();
            var categoriaExiste = await repositorioCategoria.ObtenerCategoriaPorId(categoria.Id, usuarioId);
            if (!ModelState.IsValid)
            {
                return View(categoria);
                
            }

            if (categoriaExiste is null)
            {
                return RedirectToAction("NoEncontrado", "Home");
            }

            await repositorioCategoria.ActualizarCategoria(categoria);
            return RedirectToAction("Index");
        }
    }
}
