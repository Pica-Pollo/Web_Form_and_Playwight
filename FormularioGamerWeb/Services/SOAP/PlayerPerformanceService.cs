using CoreWCF;
using FormularioGamerWeb.Contracts.SOAP.DataContracts;
using FormularioGamerWeb.Contracts.SOAP.ServiceContracts;
using FormularioGamerWeb.Helpers;
using FormularioGamerWeb.Models;

namespace FormularioGamerWeb.Services.SOAP
{
    /// <summary>
    /// Implementación del servicio SOAP de análisis de desempeño.
    /// 
    /// CARACTERÍSTICAS PROFESIONALES:
    /// - Genera WSDL válido automáticamente
    /// - Serializa resultados como XML SOAP
    /// - Implementa lógica de negocio compleja sin acceder a BD
    /// - Operaciones stateless y thread-safe
    /// - Manejo de errores con FaultContract
    /// 
    /// IMPORTANTE: Este servicio NO:
    /// - Accede a la base de datos
    /// - Realiza CRUD
    /// - Consulta datos persistentes
    /// - Solo recibe datos, analiza, devuelve resultados
    /// </summary>
    [ServiceBehavior(
        Name = "PlayerPerformanceAnalysisService",
        Namespace = "http://formulariogamer.com/2026/08")]
    public class PlayerPerformanceService : IPlayerPerformanceService
    {
        private readonly PerformanceCalculator _calculator;
        private readonly ILogger<PlayerPerformanceService> _logger;

        public PlayerPerformanceService(PerformanceCalculator calculator, ILogger<PlayerPerformanceService> logger)
        {
            _calculator = calculator;
            _logger = logger;
        }

        /// <summary>
        /// Operación principal: Análisis completo del desempeño del jugador.
        /// </summary>
        public PlayerPerformanceResult AnalyzePlayerPerformance(RegistroJugador jugador)
        {
            try
            {
                _logger.LogInformation($"Iniciando análisis para jugador: {jugador.Email}");

                // Validar entrada
                if (jugador == null)
                    throw new ArgumentNullException(nameof(jugador));

                if (string.IsNullOrWhiteSpace(jugador.Email))
                    throw new ArgumentException("El email del jugador es requerido");

                // 1. Calcular Skill Index
                int skillIndex = _calculator.CalculateSkillIndex(jugador);

                // 2. Calcular Win Rate
                decimal winRate = _calculator.CalculateWinRate(jugador, skillIndex);

                // 3. Determinar Nivel
                string nivel = _calculator.DetermineLevelBySkill(skillIndex);

                // 4. Clasificación profesional
                string clasificacion = _calculator.DetermineClassification(skillIndex, jugador);

                // 5. Dificultad recomendada
                string dificultad = _calculator.RecommendDifficulty(skillIndex);

                // 6. Género recomendado
                string generoRec = _calculator.RecommendGenre(jugador, skillIndex);

                // 7. Generar recomendaciones
                var recomendaciones = _calculator.GenerateRecommendations(jugador, skillIndex);

                // 8. Análisis detallado
                string analisisDetallado = GenerateDetailedAnalysis(jugador, skillIndex, winRate);

                // 9. Puntuación general (0-100)
                int puntuacionGeneral = (int)((skillIndex + (winRate * 100)) / 2);

                // 10. Crear resultado
                var resultado = new PlayerPerformanceResult
                {
                    SkillIndex = skillIndex,
                    WinRate = winRate,
                    Nivel = nivel,
                    Clasificacion = clasificacion,
                    GeneroRecomendado = generoRec,
                    DificultadRecomendada = dificultad,
                    Recomendaciones = recomendaciones,
                    AnalisisDetallado = analisisDetallado,
                    PuntuacionGeneral = puntuacionGeneral,
                    FechaAnalisis = DateTime.UtcNow,
                    IdAnalisis = Guid.NewGuid().ToString()
                };

                _logger.LogInformation($"Análisis completado para {jugador.Email} - SkillIndex: {skillIndex}");
                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en AnalyzePlayerPerformance: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Operación lightweight: Solo calcula skill index.
        /// </summary>
        public int CalculateSkillIndex(int experienciaAños, string generoFavorito, int edad)
        {
            try
            {
                // Crear objeto simulado para cálculo
                var jugador = new RegistroJugador
                {
                    NivelExperiencia = experienciaAños * 10, // Convertir años a escala
                    Genero = generoFavorito,
                    FechaNacimiento = DateTime.Today.AddYears(-edad)
                };

                return _calculator.CalculateSkillIndex(jugador);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en CalculateSkillIndex: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Retorna recomendaciones específicas de género.
        /// </summary>
        public List<string> GetGenreRecommendations(RegistroJugador jugador)
        {
            try
            {
                if (jugador == null)
                    throw new ArgumentNullException(nameof(jugador));

                int skillIndex = _calculator.CalculateSkillIndex(jugador);
                var recomendaciones = _calculator.GenerateRecommendations(jugador, skillIndex);

                // Filtrar solo recomendaciones de género
                return recomendaciones
                    .Where(r => r.ToLower().Contains("género") || r.ToLower().Contains("juego"))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en GetGenreRecommendations: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Valida si un jugador cumple requisitos para un rango específico.
        /// </summary>
        public bool ValidatePlayerForRank(RegistroJugador jugador, string rankTarget)
        {
            try
            {
                if (jugador == null)
                    throw new ArgumentNullException(nameof(jugador));

                if (string.IsNullOrWhiteSpace(rankTarget))
                    throw new ArgumentException("Rango objetivo requerido");

                int skillIndex = _calculator.CalculateSkillIndex(jugador);
                string nivelActual = _calculator.DetermineLevelBySkill(skillIndex);

                // Mapeo de niveles a puntuación mínima
                var requiredSkills = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
                {
                    { "Novato", 0 },
                    { "Intermedio", 40 },
                    { "Avanzado", 55 },
                    { "Experto", 70 },
                    { "Profesional", 85 }
                };

                if (!requiredSkills.TryGetValue(rankTarget, out int requiredSkill))
                    throw new ArgumentException($"Rango desconocido: {rankTarget}");

                return skillIndex >= requiredSkill;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en ValidatePlayerForRank: {ex.Message}");
                throw;
            }
        }

        // ============================================================
        // MÉTODOS PRIVADOS
        // ============================================================

        /// <summary>
        /// Genera análisis textual detallado del jugador.
        /// </summary>
        private string GenerateDetailedAnalysis(RegistroJugador jugador, int skillIndex, decimal winRate)
        {
            var analysis = new System.Text.StringBuilder();

            analysis.AppendLine($"=== ANÁLISIS DETALLADO DE DESEMPEÑO ===");
            analysis.AppendLine($"Jugador: {jugador.Nombre} {jugador.Apellido}");
            analysis.AppendLine($"Fecha: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
            analysis.AppendLine();

            analysis.AppendLine($"ÍNDICES PRINCIPALES:");
            analysis.AppendLine($"  • Skill Index: {skillIndex}/100");
            analysis.AppendLine($"  • Win Rate Estimado: {winRate:P1}");
            analysis.AppendLine();

            analysis.AppendLine($"PERFIL DEL JUGADOR:");
            analysis.AppendLine($"  • Género : {jugador.Genero}");
            analysis.AppendLine($"  • Plataforma Principal: {jugador.PlataformaFavorita}");
            analysis.AppendLine($"  • Nivel de Experiencia: {jugador.NivelExperiencia}%");
            analysis.AppendLine();

            // Análisis interpretativo
            if (skillIndex >= 70)
            {
                analysis.AppendLine($"EVALUACIÓN: Este jugador tiene un alto nivel de habilidad.");
                analysis.AppendLine($"Potencial: Alto. Capaz de competir a nivel semi-profesional.");
            }
            else if (skillIndex >= 50)
            {
                analysis.AppendLine($"EVALUACIÓN: Jugador con habilidades sólidas y en constante mejora.");
                analysis.AppendLine($"Potencial: Moderado. Mejora posible con entrenamiento dedicado.");
            }
            else if (skillIndex >= 30)
            {
                analysis.AppendLine($"EVALUACIÓN: Jugador en fase de aprendizaje activo.");
                analysis.AppendLine($"Potencial: Hay mucho espacio para crecer. Práctica consistente es clave.");
            }
            else
            {
                analysis.AppendLine($"EVALUACIÓN: Jugador principiante o iniciándose en este género.");
                analysis.AppendLine($"Potencial: Alto si se compromete con el aprendizaje sistemático.");
            }

            return analysis.ToString();
        }
    }
}
