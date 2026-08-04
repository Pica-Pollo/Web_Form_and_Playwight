using System.Runtime.Serialization;

namespace FormularioGamerWeb.Contracts.SOAP.DataContracts
{
    /// <summary>
    /// Resultado del análisis de desempeño del jugador.
    /// Este objeto se serializa como XML en la respuesta SOAP.
    /// </summary>
    [DataContract(Name = "PlayerPerformanceResult", Namespace = "http://formulariogamer.com/2026/08")]
    public class PlayerPerformanceResult
    {
        /// <summary>
        /// Índice de habilidad calculado (0-100).
        /// Basado en: experiencia, género favorito, plataforma, edad.
        /// </summary>
        [DataMember(Order = 1)]
        public int SkillIndex { get; set; }

        /// <summary>
        /// Tasa de victoria estimada en %.
        /// Simulación pseudoaleatoria determinística basada en datos del jugador.
        /// </summary>
        [DataMember(Order = 2)]
        public decimal WinRate { get; set; }

        /// <summary>
        /// Nivel estimado del jugador.
        /// Valores: "Novato", "Intermedio", "Avanzado", "Experto", "Profesional"
        /// </summary>
        [DataMember(Order = 3)]
        public string Nivel { get; set; } = string.Empty;

        /// <summary>
        /// Clasificación profesional del jugador.
        /// Basada en el SkillIndex y patrón de juego.
        /// </summary>
        [DataMember(Order = 4)]
        public string Clasificacion { get; set; } = string.Empty;

        /// <summary>
        /// Género recomendado según perfil: "Acción", "Estrategia", "RPG", "Deportes", "Puzzle"
        /// </summary>
        [DataMember(Order = 5)]
        public string GeneroRecomendado { get; set; } = string.Empty;

        /// <summary>
        /// Dificultad recomendada en videojuegos.
        /// Valores: "Fácil", "Normal", "Difícil", "Extremo"
        /// </summary>
        [DataMember(Order = 6)]
        public string DificultadRecomendada { get; set; } = string.Empty;

        /// <summary>
        /// Lista de recomendaciones personalizadas para mejorar desempeño.
        /// </summary>
        [DataMember(Order = 7)]
        public List<string> Recomendaciones { get; set; } = new List<string>();

        /// <summary>
        /// Análisis detallado con puntos fuertes y débiles.
        /// </summary>
        [DataMember(Order = 8)]
        public string AnalisisDetallado { get; set; } = string.Empty;

        /// <summary>
        /// Puntuación general (0-100) que resume el análisis.
        /// </summary>
        [DataMember(Order = 9)]
        public int PuntuacionGeneral { get; set; }

        /// <summary>
        /// Momento de generación del análisis (ISO 8601 UTC).
        /// </summary>
        [DataMember(Order = 10)]
        public DateTime FechaAnalisis { get; set; }

        /// <summary>
        /// Identificador único del análisis para trazabilidad.
        /// </summary>
        [DataMember(Order = 11)]
        public string IdAnalisis { get; set; } = string.Empty;
    }
}
