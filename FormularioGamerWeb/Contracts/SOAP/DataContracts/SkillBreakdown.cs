using System.Runtime.Serialization;

namespace FormularioGamerWeb.Contracts.SOAP.DataContracts
{
    /// <summary>
    /// Información adicional de habilidades desglosadas.
    /// Envía al cliente información detallada de cada aspecto del análisis.
    /// </summary>
    [DataContract(Name = "SkillBreakdown", Namespace = "http://formulariogamer.com/2026/08")]
    public class SkillBreakdown
    {
        /// <summary>
        /// Habilidad en reflexión y tiempo de reacción.
        /// Escala 0-100.
        /// </summary>
        [DataMember(Order = 1)]
        public int ReflexeScore { get; set; }

        /// <summary>
        /// Habilidad estratégica y toma de decisiones.
        /// Escala 0-100.
        /// </summary>
        [DataMember(Order = 2)]
        public int StrategicScore { get; set; }

        /// <summary>
        /// Experiencia acumulada (años jugando).
        /// </summary>
        [DataMember(Order = 3)]
        public decimal ExperienceYears { get; set; }

        /// <summary>
        /// Puntuación de consistencia (qué tan estable es el jugador).
        /// Escala 0-100.
        /// </summary>
        [DataMember(Order = 4)]
        public int ConsistencyScore { get; set; }

        /// <summary>
        /// Puntuación de adaptabilidad a diferentes géneros.
        /// Escala 0-100.
        /// </summary>
        [DataMember(Order = 5)]
        public int AdaptabilityScore { get; set; }
    }
}
