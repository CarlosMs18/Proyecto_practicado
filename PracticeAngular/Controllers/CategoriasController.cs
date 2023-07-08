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
        public ActionResult Index() {
            return View();
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
    }
}
