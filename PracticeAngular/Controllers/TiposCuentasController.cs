using Microsoft.AspNetCore.Mvc;

namespace PracticeAngular.Controllers
{
    public class TiposCuentas : Controller
    {

        public IActionResult Index()
        {
            return View();  
        }

        [HttpGet]
        public IActionResult Crear()
        {

            return View();
        }


        [HttpPost]
        public IActionResult Crear(int xd)
        {
            return View();
        }
    }
}
