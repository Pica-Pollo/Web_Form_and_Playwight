using FormularioGamerWeb.Models;

namespace FormularioGamerWeb.Helpers
{
    /// <summary>
    /// Motor central de cálculos para análisis de desempeño.
    /// Implementa toda la lógica matemática y algoritmos de negocio.
    /// 
    /// IMPORTANTE: Esta clase NO accede a la base de datos.
    /// Solo realiza cálculos basados en datos de entrada.
    /// </summary>
    public class PerformanceCalculator
    {
        /// <summary>
        /// Calcula el Skill Index (0-100) basado en múltiples factores.
        /// 
        /// Fórmula ponderada:
        /// SkillIndex = (ExperienciaScore * 0.35) + (GeneroScore * 0.25) + 
        ///              (PlataformaScore * 0.20) + (EdadScore * 0.20)
        /// </summary>
        public int CalculateSkillIndex(RegistroJugador jugador)
        {
            // 1. Puntuación por experiencia (0-35)
            // Cada año de experiencia suma 3.5 puntos (máx 10 años = 35 puntos)
            int experienciaScore = Math.Min((jugador.NivelExperiencia / 10) * 35, 35);

            // 2. Puntuación por género favorito (0-25)
            int generoScore = CalculateGenreScore(jugador.Genero);

            // 3. Puntuación por plataforma (0-20)
            int plataformaScore = CalculatePlatformScore(jugador.PlataformaFavorita);

            // 4. Puntuación por edad (0-20)
            // Óptimo: 15-35 años (20 puntos)
            // Decae bajo 15 y sobre 35
            int edad = DateTime.Today.Year - jugador.FechaNacimiento.Year;
            int edadScore = CalculateAgeScore(edad);

            // Cálculo final ponderado
            int skillIndex = (int)((experienciaScore * 0.35) + 
                                   (generoScore * 0.25) + 
                                   (plataformaScore * 0.20) + 
                                   (edadScore * 0.20));

            return Math.Max(0, Math.Min(100, skillIndex)); // Clamped 0-100
        }

        /// <summary>
        /// Calcula Win Rate simulado de forma determinística.
        /// Usa el email como seed para reproducibilidad.
        /// </summary>
        public decimal CalculateWinRate(RegistroJugador jugador, int skillIndex)
        {
            // Seed determinístico basado en email (siempre produce mismo resultado)
            int seed = Math.Abs(jugador.Email?.GetHashCode() ?? 0);
            var random = new Random(seed);

            // Base: skill index como porcentaje
            decimal baseWinRate = skillIndex / 100m;

            // Variación ±15% basada en seed
            decimal variation = (decimal)(random.NextDouble() * 0.30d) - 0.15m;
            decimal finalWinRate = baseWinRate + variation;

            // Clamp entre 5% y 95%
            return Math.Max(0.05m, Math.Min(0.95m, finalWinRate));
        }

        /// <summary>
        /// Determina el nivel del jugador según su skill.
        /// </summary>
        public string DetermineLevelBySkill(int skillIndex)
        {
            return skillIndex switch
            {
                >= 85 => "Profesional",
                >= 70 => "Experto",
                >= 55 => "Avanzado",
                >= 40 => "Intermedio",
                _ => "Novato"
            };
        }

        /// <summary>
        /// Asigna clasificación profesional basada en análisis completo.
        /// </summary>
        public string DetermineClassification(int skillIndex, RegistroJugador jugador)
        {
            // En la versión actual, RegistroJugador solo tiene PlataformaFavorita (string)
            // No tiene una colección PlataformasJuego
            bool esCompetitivo = !string.IsNullOrEmpty(jugador.PlataformaFavorita) && 
                (jugador.PlataformaFavorita.ToLower().Contains("online") || 
                 jugador.PlataformaFavorita.ToLower().Contains("competitivo"));

            string baseLevel = DetermineLevelBySkill(skillIndex);

            // Si es competitivo y tiene buen skill, añade sufijo
            if (esCompetitivo && skillIndex >= 60)
            {
                return $"{baseLevel} (Competitivo)";
            }

            return baseLevel;
        }

        /// <summary>
        /// Recomienda dificultad basada en skill.
        /// </summary>
        public string RecommendDifficulty(int skillIndex)
        {
            return skillIndex switch
            {
                >= 80 => "Extremo",
                >= 60 => "Difícil",
                >= 40 => "Normal",
                _ => "Fácil"
            };
        }

        /// <summary>
        /// Recomienda género basado en perfil del jugador.
        /// </summary>
        public string RecommendGenre(RegistroJugador jugador, int skillIndex)
        {
            // Si ya tiene un género favorito, usa ese como base
            string generoActual = jugador.Genero ?? "Acción";

            // Pero si el skill es muy alto, sugiere retos mayores
            if (skillIndex >= 75 && generoActual != "Estrategia")
            {
                return "Estrategia"; // Requiere más pensamiento
            }

            if (skillIndex >= 85)
            {
                return "RPG Competitivo"; // Máximo desafío
            }

            return generoActual;
        }

        /// <summary>
        /// Genera recomendaciones personalizadas de mejora.
        /// </summary>
        public List<string> GenerateRecommendations(RegistroJugador jugador, int skillIndex)
        {
            var recomendaciones = new List<string>();
            int edad = DateTime.Today.Year - jugador.FechaNacimiento.Year;

            // Recomendación por nivel
            if (skillIndex < 30)
            {
                recomendaciones.Add("Enfócate en aprender la mecánica básica del juego. Prueba tutoriales.");
                recomendaciones.Add("Juega en dificultad Fácil para ganar confianza y entender los controles.");
                recomendaciones.Add("Dedica 30 minutos diarios de práctica consistente.");
            }
            else if (skillIndex < 50)
            {
                recomendaciones.Add("Ya dominas los conceptos básicos. Intenta estrategias más avanzadas.");
                recomendaciones.Add("Aumenta gradualmente la dificultad de los juegos.");
                recomendaciones.Add("Estudia a jugadores profesionales para aprender técnicas avanzadas.");
            }
            else if (skillIndex < 75)
            {
                recomendaciones.Add("Eres un jugador competitivo. Considera participar en torneos online.");
                recomendaciones.Add("Especializarte en un género podría llevarte al siguiente nivel.");
                recomendaciones.Add("Trabaja en tu consistencia mental durante sesiones largas.");
            }
            else
            {
                recomendaciones.Add("Tienes potencial profesional. Considera el streaming o esports.");
                recomendaciones.Add("Tu mayor desafío es la innovación en estrategia. Experimenta constantemente.");
                recomendaciones.Add("Entrenar con equipos de ligas profesionales aceleraría tu desarrollo.");
            }

            // Recomendación por edad
            if (edad < 18)
            {
                recomendaciones.Add("A tu edad, tu reflejo está en su pico máximo. Aprovéchalo.");
            }
            else if (edad > 40)
            {
                recomendaciones.Add("Mantén tu agilidad mental con juegos estratégicos que requieren pensamiento.");
                recomendaciones.Add("Tu experiencia es tu mejor activo. Usa tu conocimiento acumulado.");
            }

            // Recomendación por plataforma
            // Nota: En el modelo actual solo hay una plataforma favorita, no colección

            return recomendaciones;
        }

        // ============================================================
        // MÉTODOS PRIVADOS DE CÁLCULO
        // ============================================================

        private int CalculateGenreScore(string? genero)
        {
            return genero?.ToLower() switch
            {
                "masculino" => 20,        // Valor neutral
                "femenino" => 20,         // Valor neutral
                "otro" => 20,             // Valor neutral
                _ => 15                   // Genérico (en realidad es Género de jugador, no género de juego)
            };
        }

        private int CalculatePlatformScore(string? plataforma)
        {
            if (string.IsNullOrEmpty(plataforma))
                return 10;

            return plataforma.ToLower() switch
            {
                "pc" => 20,              // Controles más precisos
                "consola" => 19,         // Muy competitivo
                "tableta" => 12,         // Limitaciones de hardware
                "móvil" => 11,           // Pantalla pequeña
                _ => 10
            };
        }

        private int CalculateAgeScore(int edad)
        {
            // Óptimo entre 18-30 años
            if (edad >= 18 && edad <= 30)
                return 20;

            if (edad >= 15 && edad < 18)
                return 18; // Jóvenes tienen gran potencial

            if (edad > 30 && edad <= 40)
                return 15; // Experiencia compensa algo de reflexividad

            if (edad > 40)
                return 10; // Declina pero no desaparece

            return 5; // Menores de 15
        }
    }
}
