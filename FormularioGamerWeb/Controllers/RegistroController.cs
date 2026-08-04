using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FormularioGamerWeb.Data;
using FormularioGamerWeb.Models;
using FormularioGamerWeb.Contracts.SOAP.ServiceContracts;
using FormularioGamerWeb.Services.REST;
using System.Security.Cryptography;
using System.Text;

namespace FormularioGamerWeb.Controllers
{
    /// <summary>
    /// Controlador del formulario de registro de jugador.
    /// Funcionalidades:
    /// 1. Mostrar formulario vacío (GET)
    /// 2. Procesar registro (POST)
    /// 3. Confirmar registro (confirmación)
    /// 4. Analizar desempeño (integrar SOAP)
    /// 5. Mostrar clima (integrar API REST)
    /// </summary>
    public class RegistroController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IPlayerPerformanceService _performanceService;
        private readonly IWeatherClient _weatherClient;
        private readonly ILogger<RegistroController> _logger;

        public RegistroController(
            ApplicationDbContext context, 
            IWebHostEnvironment environment,
            IPlayerPerformanceService performanceService,
            IWeatherClient weatherClient,
            ILogger<RegistroController> logger)
        {
            _context = context;
            _environment = environment;
            _performanceService = performanceService;
            _weatherClient = weatherClient;
            _logger = logger;
        }

        // ============================================================
        // GET: /Registro/Index  → Muestra el formulario vacío
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
        // POST: /Registro/Index → Procesa el envío del formulario
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
            // VALIDAR QUE SE GUARDÓ CORRECTAMENTE
            // ----------------------------------------------------------
            var registroGuardado = await _context.RegistrosJugadores
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == modelo.Id);

            if (registroGuardado == null)
            {
                TempData["MensajeError"] = "Ocurrió un error al guardar. Intenta nuevamente.";
                return View(modelo);
            }

            _logger.LogInformation($"Nuevo jugador registrado: {modelo.Email}");

            // Redirige a la página de confirmación (patrón Post-Redirect-Get)
            return RedirectToAction("Confirmacion", new { id = modelo.Id });
        }

        // ============================================================
        // GET: /Registro/Confirmacion/5 → Página de éxito
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Confirmacion(int id)
        {
            var registro = await _context.RegistrosJugadores.FindAsync(id);
            if (registro == null)
            {
                TempData["MensajeError"] = "No se encontró el registro.";
                return RedirectToAction("Index");
            }

            return View(registro);
        }

        // ============================================================
        // GET: /Registro/Lista → Lista de jugadores registrados
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            var jugadores = await _context.RegistrosJugadores
                .OrderByDescending(r => r.FechaRegistro)
                .ToListAsync();

            return View(jugadores);
        }

        // ============================================================
        // POST: /Registro/AnalyzePerformance → Llamar SOAP Service
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> AnalyzePerformance(int id)
        {
            try
            {
                var jugador = await _context.RegistrosJugadores.FindAsync(id);
                if (jugador == null)
                {
                    return Json(new { success = false, message = "Jugador no encontrado" });
                }

                _logger.LogInformation($"Analizando desempeño para: {jugador.Email}");

                // Llamar al servicio SOAP
                var resultado = _performanceService.AnalyzePlayerPerformance(jugador);

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        resultado.SkillIndex,
                        resultado.WinRate,
                        resultado.Nivel,
                        resultado.Clasificacion,
                        resultado.GeneroRecomendado,
                        resultado.DificultadRecomendada,
                        resultado.Recomendaciones,
                        resultado.AnalisisDetallado,
                        resultado.PuntuacionGeneral,
                        resultado.IdAnalisis
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en AnalyzePerformance: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // GET: /Registro/GetWeather?lat=...&lon=...
        // Obtener datos climáticos (API REST externa)
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetWeather(double lat, double lon)
        {
            try
            {
                var weather = await _weatherClient.GetCurrentWeatherAsync(lat, lon);

                if (weather == null)
                {
                    return Json(new { success = false, message = "No se pudieron obtener datos climáticos" });
                }

                return Json(new
                {
                    success = true,
                    data = new
                    {
                        temperature = weather.Current?.Temperature,
                        windSpeed = weather.Current?.WindSpeed,
                        weatherCode = weather.Current?.WeatherCode,
                        description = weather.GetWeatherDescription(),
                        timezone = weather.Coordinates?.Timezone,
                        latitude = weather.Coordinates?.Latitude,
                        longitude = weather.Coordinates?.Longitude
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en GetWeather: {ex.Message}");
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // GET: /Registro/Api/GetById/5 → Obtener jugador por ID (JSON)
        // Útil para validación con Playwright
        // ============================================================
        [HttpGet("registro/api/getbyid/{id}")]
        public async Task<IActionResult> ApiGetById(int id)
        {
            var registro = await _context.RegistrosJugadores.FindAsync(id);
            if (registro == null)
                return NotFound(new { message = "Jugador no encontrado" });

            return Json(new
            {
                id = registro.Id,
                nombre = registro.Nombre,
                apellido = registro.Apellido,
                email = registro.Email,
                genero = registro.Genero,
                nivel = registro.NivelExperiencia,
                fechaRegistro = registro.FechaRegistro
            });
        }

        // ============================================================
        // GET: /Registro/Api/Count → Contar registros totales
        // ============================================================
        [HttpGet("registro/api/count")]
        public async Task<IActionResult> ApiCount()
        {
            var count = await _context.RegistrosJugadores.CountAsync();
            return Json(new { totalJugadores = count });
        }

        // ============================================================
        // MÉTODO PRIVADO: Hash de contraseña
        // ============================================================
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
