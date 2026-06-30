using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FormularioGamerWeb.Models
{
    /// <summary>
    /// Representa un "Jugador" registrado a través del formulario.
    /// Esta clase es el MODELO en el patrón MVC: define la forma de los datos
    /// y las reglas de validación. Entity Framework Core usa esta clase para
    /// crear/mapear la tabla "RegistrosJugadores" en SQL Server.
    /// </summary>
    public class RegistroJugador
    {
        // Clave primaria. Entity Framework la reconoce automáticamente
        // por convención al llamarse "Id".
        [Key]
        public int Id { get; set; }

        // ---------------- 1. TextBox ----------------
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres")]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; } = string.Empty;

        // ---------------- 2. TextBox ----------------
        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "El apellido debe tener entre 2 y 100 caracteres")]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; } = string.Empty;

        // ---------------- 3. Email ----------------
        [Required(ErrorMessage = "El correo es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato de correo no es válido")]
        [Display(Name = "Correo Electrónico")]
        public string Email { get; set; } = string.Empty;

        // ---------------- 4. Password ----------------
        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        // ---------------- 5. Number ----------------
        [Required(ErrorMessage = "La edad es obligatoria")]
        [Range(18, 99, ErrorMessage = "La edad debe estar entre 18 y 99 años")]
        [Display(Name = "Edad")]
        public int Edad { get; set; }

        // ---------------- 6. Date Picker ----------------
        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        // ---------------- 7. Time Picker ----------------
        [Display(Name = "Horario Preferido de Juego")]
        public TimeSpan? HoraPreferida { get; set; }

        // ---------------- 8. TextArea ----------------
        [StringLength(500, ErrorMessage = "La biografía no puede exceder 500 caracteres")]
        [Display(Name = "Biografía / Descripción")]
        public string? Biografia { get; set; }

        // ---------------- 9. Radio Button ----------------
        [Required(ErrorMessage = "Selecciona un género")]
        [Display(Name = "Género")]
        public string Genero { get; set; } = string.Empty;

        // ---------------- 10. Select (Dropdown) ----------------
        [Required(ErrorMessage = "Selecciona un país")]
        [Display(Name = "País")]
        public string Pais { get; set; } = string.Empty;

        // ---------------- 11. Select (Dropdown) ----------------
        [Required(ErrorMessage = "Selecciona una plataforma")]
        [Display(Name = "Plataforma Favorita")]
        public string PlataformaFavorita { get; set; } = string.Empty;

        // ---------------- 12. CheckBox ----------------
        [Display(Name = "Acepto los Términos y Condiciones")]
        public bool AceptaTerminos { get; set; }

        // ---------------- 13. Switch / Toggle ----------------
        [Display(Name = "Recibir Notificaciones")]
        public bool RecibirNotificaciones { get; set; }

        // ---------------- 14. Range Slider ----------------
        [Range(1, 100, ErrorMessage = "El nivel de experiencia debe estar entre 1 y 100")]
        [Display(Name = "Nivel de Experiencia")]
        public int NivelExperiencia { get; set; } = 50;

        // ---------------- 15. Color Picker ----------------
        [Display(Name = "Color Favorito")]
        [RegularExpression("^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color no válido")]
        public string ColorFavorito { get; set; } = "#00F0FF";

        // ---------------- 16. File Upload ----------------
        // No se mapea a la BD directamente: solo guardamos la RUTA del archivo subido.
        [NotMapped]
        [Display(Name = "Avatar")]
        public IFormFile? AvatarArchivo { get; set; }

        // Ruta del archivo ya guardada en disco (esto SÍ se guarda en BD)
        public string? AvatarRuta { get; set; }

        // ---------------- 17. Audio Control ----------------
        // Controla si el efecto de sonido (Web Audio API) estaba activado al enviar
        [Display(Name = "Sonido Activado")]
        public bool SonidoActivado { get; set; } = true;

        // ---------------- Metadatos ----------------
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}