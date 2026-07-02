using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormularioGamerWeb.Data;
using FormularioGamerWeb.Models;
using System.Security.Cryptography;
using System.Text;

namespace FormularioGamerWeb.Controllers
{
    /// <summary>
    /// Controlador del formulario de registro de jugador.
    /// Maneja: mostrar el formulario (GET), procesar el envío (POST),
    /// y exponer endpoints JSON simples para que Playwright (u otras
    /// herramientas) puedan VERIFICAR que los datos quedaron guardados.
    /// </summary>
    public class RegistroController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RegistroController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ============================================================
        // GET: /Registro/Index  -> Muestra el formulario vacío
        // ============================================================
        [HttpGet]
        public IActionResult Index()
        {
            var modelo = new RegistroJugador
            {
                FechaNacimiento = DateTime.Today.AddYears(-18),
                NivelExperiencia = 50,
                ColorFavorito = "#00F0FF"
            };
            return View(modelo);
        }

        // ============================================================
        // POST: /Registro/Index -> Procesa el envío del formulario
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(RegistroJugador modelo)
        {
            // ----------------------------------------------------------
            // VALIDACIÓN 1: Anotaciones de datos del modelo
            // (Required, Range, EmailAddress, etc. definidas en el Model)
            // ----------------------------------------------------------
            if (!ModelState.IsValid)
            {
                TempData["MensajeError"] = "Revisa los campos marcados, hay datos inválidos o faltantes.";
                return View(modelo);
            }

            // ======================================================================
            // AGREGA ESTE BLOQUE AQUÍ PARA CORREGIR EL CASO 8:
            // ======================================================================
            // VALIDACIÓN 1B: La fecha de nacimiento no puede ser una fecha futura
            // ----------------------------------------------------------------------
            if (modelo.FechaNacimiento > DateTime.Today)
            {
                ModelState.AddModelError("FechaNacimiento", "La fecha de nacimiento no puede ser una fecha futura.");
                TempData["MensajeError"] = "Revisa los campos marcados, hay datos inválidos o faltantes.";
                return View(modelo);
            }

            // ----------------------------------------------------------
            // VALIDACIÓN 2: Email único (consulta directa a la BD)
            // ----------------------------------------------------------
            bool emailExiste = await _context.RegistrosJugadores
                .AnyAsync(r => r.Email == modelo.Email);

            if (emailExiste)
            {
                ModelState.AddModelError("Email", "Este correo ya está registrado");
                TempData["MensajeError"] = "El correo ya está registrado.";
                return View(modelo);
            }

            // ----------------------------------------------------------
            // VALIDACIÓN 3: Términos y condiciones obligatorios
            // ----------------------------------------------------------
            if (!modelo.AceptaTerminos)
            {
                ModelState.AddModelError("AceptaTerminos", "Debes aceptar los términos y condiciones");
                TempData["MensajeError"] = "Debes aceptar los términos y condiciones.";
                return View(modelo);
            }

            // ----------------------------------------------------------
            // PROCESAR ARCHIVO (Avatar) si fue subido
            // ----------------------------------------------------------
            if (modelo.AvatarArchivo != null && modelo.AvatarArchivo.Length > 0)
            {
                var extensionesValidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(modelo.AvatarArchivo.FileName).ToLowerInvariant();

                if (!extensionesValidas.Contains(extension))
                {
                    ModelState.AddModelError("AvatarArchivo", "Formato de archivo no válido. Usa JPG, PNG, GIF o WEBP.");
                    TempData["MensajeError"] = "Formato de archivo no válido.";
                    return View(modelo);
                }

                if (modelo.AvatarArchivo.Length > 5 * 1024 * 1024) // 5MB
                {
                    ModelState.AddModelError("AvatarArchivo", "El archivo no puede superar 5MB.");
                    TempData["MensajeError"] = "El archivo es demasiado grande (máx. 5MB).";
                    return View(modelo);
                }

                var carpetaUploads = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(carpetaUploads);

                var nombreUnico = $"avatar_{DateTime.Now.Ticks}{extension}";
                var rutaCompleta = Path.Combine(carpetaUploads, nombreUnico);

                using (var stream = new FileStream(rutaCompleta, FileMode.Create))
                {
                    await modelo.AvatarArchivo.CopyToAsync(stream);
                }

                modelo.AvatarRuta = $"/uploads/{nombreUnico}";
            }

            // ----------------------------------------------------------
            // HASHEAR LA CONTRASEÑA (nunca se guarda en texto plano)
            // ----------------------------------------------------------
            modelo.Password = HashPassword(modelo.Password);
            modelo.FechaRegistro = DateTime.Now;

            // ----------------------------------------------------------
            // GUARDAR EN BASE DE DATOS
            // ----------------------------------------------------------
            _context.RegistrosJugadores.Add(modelo);
            await _context.SaveChangesAsync();

            // ----------------------------------------------------------
            // VALIDAR QUE SE GUARDÓ CORRECTAMENTE (lectura de confirmación)
            // ----------------------------------------------------------
            var registroGuardado = await _context.RegistrosJugadores
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == modelo.Id);

            if (registroGuardado == null)
            {
                TempData["MensajeError"] = "Ocurrió un error al guardar. Intenta nuevamente.";
                return View(modelo);
            }

            // Redirige a la página de confirmación (patrón Post-Redirect-Get)
            return RedirectToAction("Confirmacion", new { id = modelo.Id });
        }

        // ============================================================
        // GET: /Registro/Confirmacion/5 -> Página de éxito
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Confirmacion(int id)
        {
            var registro = await _context.RegistrosJugadores.FindAsync(id);
            if (registro == null)
            {
                return NotFound();
            }
            return View(registro);
        }

        // ============================================================
        // GET: /Registro/Lista -> Lista todos los registros (para revisar)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var registros = await _context.RegistrosJugadores
                .AsNoTracking()
                .OrderByDescending(r => r.FechaRegistro)
                .ToListAsync();
            return View(registros);
        }

        // ============================================================
        // API JSON simple: GET /Registro/ApiUsuarios
        // Útil para que Playwright (u otra herramienta) verifique
        // por código que los registros quedaron en la base de datos,
        // sin necesidad de abrir SSMS.
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> ApiUsuarios()
        {
            var registros = await _context.RegistrosJugadores
                .AsNoTracking()
                .OrderByDescending(r => r.FechaRegistro)
                .Select(r => new
                {
                    r.Id,
                    r.Nombre,
                    r.Apellido,
                    r.Email,
                    r.Pais,
                    r.Genero,
                    r.FechaRegistro
                })
                .ToListAsync();

            return Json(new { success = true, count = registros.Count, data = registros });
        }

        [HttpGet]
        public async Task<IActionResult> ApiUsuarioPorEmail(string email)
        {
            var registro = await _context.RegistrosJugadores
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Email == email);

            if (registro == null)
            {
                return Json(new { success = false, message = "No encontrado" });
            }

            return Json(new
            {
                success = true,
                data = new
                {
                    registro.Id,
                    registro.Nombre,
                    registro.Apellido,
                    registro.Email,
                    registro.Pais,
                    registro.FechaRegistro
                }
            });
        }

        // ============================================================
        // Hash simple de contraseña con SHA-256.
        // Nota educativa: para producción real se recomienda BCrypt
        // o ASP.NET Core Identity, que añaden "salt" automáticamente.
        // ============================================================
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}