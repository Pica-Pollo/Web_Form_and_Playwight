using Microsoft.AspNetCore.Mvc;

namespace FormularioGamerWeb.Controllers
{
    /// <summary>
    /// Controlador "Home" mínimo. Toda la lógica real vive en RegistroController.
    /// Lo dejamos solo para manejar la página de error genérica de ASP.NET Core.
    /// </summary>
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Registro");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}