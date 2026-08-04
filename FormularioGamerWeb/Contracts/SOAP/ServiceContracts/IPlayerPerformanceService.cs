using CoreWCF;
using FormularioGamerWeb.Contracts.SOAP.DataContracts;
using FormularioGamerWeb.Models;

namespace FormularioGamerWeb.Contracts.SOAP.ServiceContracts
{
    /// <summary>
    /// Contrato SOAP para el servicio de análisis de desempeño del jugador.
    /// Define las operaciones disponibles y sus firmas.
    /// 
    /// Este servicio implementa lógica de negocio PROFESIONAL:
    /// - NO accede a la base de datos
    /// - NO realiza operaciones CRUD
    /// - Solo recibe datos del jugador, analiza, y retorna resultados
    /// - Usa algoritmos matemáticos y reglas de negocio
    /// 
    /// Genera WSDL automáticamente para ser consumido por clientes externos.
    /// </summary>
    [ServiceContract(
        Name = "PlayerPerformanceAnalysisService",
        Namespace = "http://formulariogamer.com/2026/08",
        ConfigurationName = "IPlayerPerformanceService")]
    public interface IPlayerPerformanceService
    {
        /// <summary>
        /// Analiza el desempeño de un jugador basado en sus atributos registrados.
        /// 
        /// Entrada: Objeto RegistroJugador con todos sus datos
        /// Procedimiento:
        ///   1. Valida los datos de entrada
        ///   2. Calcula índices de habilidad usando algoritmos propios
        ///   3. Genera recomendaciones personalizadas
        ///   4. Retorna análisis completo en formato XML (SOAP)
        /// 
        /// Salida: PlayerPerformanceResult con análisis profesional
        /// 
        /// Este método implementa LÓGICA DE NEGOCIO REAL:
        /// - Skill Index: función de (experiencia, edad, género favorito, plataforma)
        /// - Win Rate: simulación con seed determinístico
        /// - Nivel: clasificación basada en ScoreGeneral
        /// - Recomendaciones: reglas de negocio personalizadas
        /// </summary>
        [OperationContract]
        PlayerPerformanceResult AnalyzePlayerPerformance(RegistroJugador jugador);

        /// <summary>
        /// Calcula solo el índice de habilidad sin análisis completo.
        /// Operación lightweight para validaciones rápidas.
        /// </summary>
        [OperationContract]
        int CalculateSkillIndex(int experienciaAños, string generoFavorito, int edad);

        /// <summary>
        /// Genera recomendaciones específicas de genero para el jugador.
        /// Útil para sugerencias personalizadas.
        /// </summary>
        [OperationContract]
        List<string> GetGenreRecommendations(RegistroJugador jugador);

        /// <summary>
        /// Valida si el perfil del jugador cumple criterios para un rango específico.
        /// </summary>
        [OperationContract]
        bool ValidatePlayerForRank(RegistroJugador jugador, string rankTarget);
    }
}
