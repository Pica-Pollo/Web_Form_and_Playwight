using System.Net.Http.Json;
using FormularioGamerWeb.Contracts.REST;

namespace FormularioGamerWeb.Services.REST
{
    /// <summary>
    /// Cliente HTTP para consumir la API REST externa.
    /// Usa: Open-Meteo Weather API
    /// 
    /// CARACTERÍSTICAS:
    /// - API Gratuita: Sin requerimiento de clave API
    /// - Pública: Disponible para cualquiera
    /// - Estable: Uptime > 99.9%
    /// - Bien documentada: https://open-meteo.com/en/docs
    /// 
    /// LÓGICA DE NEGOCIO INTEGRADA:
    /// Las condiciones climáticas afectan el rendimiento de los jugadores.
    /// Por ejemplo:
    /// - Lluvia/Tormenta: Puede afectar conexión / distracción
    /// - Temperatura extrema: Puede afectar concentración
    /// - Cielo despejado: Mejor para sesiones largas
    /// 
    /// Este servicio REST enriquece el análisis SOAP con contexto del mundo real.
    /// </summary>
    public interface IWeatherClient
    {
        /// <summary>
        /// Obtiene condiciones climáticas actuales para una ubicación específica.
        /// </summary>
        Task<WeatherConditionData?> GetCurrentWeatherAsync(double latitude, double longitude);

        /// <summary>
        /// Obtiene condiciones climáticas por ubicación aproximada (país/ciudad).
        /// Usa geocodificación.
        /// </summary>
        Task<WeatherConditionData?> GetWeatherByCityAsync(string city, string countryCode);
    }

    /// <summary>
    /// Implementación del cliente REST para Open-Meteo.
    /// </summary>
    public class WeatherClient : IWeatherClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<WeatherClient> _logger;
        private const string BaseUrl = "https://api.open-meteo.com/v1";
        private const string GeocodeUrl = "https://geocoding-api.open-meteo.com/v1";

        public WeatherClient(HttpClient httpClient, ILogger<WeatherClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene clima actual por coordenadas (Lat/Long).
        /// </summary>
        public async Task<WeatherConditionData?> GetCurrentWeatherAsync(double latitude, double longitude)
        {
            try
            {
                _logger.LogInformation($"Obteniendo clima para coordenadas: {latitude}, {longitude}");

                string url = $"{BaseUrl}/forecast?latitude={latitude}&longitude={longitude}&current=temperature_2m,weather_code,wind_speed_10m&timezone=auto";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                using var jsonDoc = System.Text.Json.JsonDocument.Parse(await response.Content.ReadAsStringAsync());
                var root = jsonDoc.RootElement;

                var weatherData = new WeatherConditionData
                {
                    Coordinates = new WeatherConditionData.LocationCoordinates
                    {
                        Latitude = latitude,
                        Longitude = longitude,
                        Timezone = root.GetProperty("timezone").GetString() ?? "UTC"
                    },
                    Current = new WeatherConditionData.CurrentWeather
                    {
                        Temperature = root.GetProperty("current").GetProperty("temperature_2m").GetDouble(),
                        WindSpeed = root.GetProperty("current").GetProperty("wind_speed_10m").GetDouble(),
                        WeatherCode = root.GetProperty("current").GetProperty("weather_code").GetInt32()
                    }
                };

                _logger.LogInformation($"Clima obtenido: {weatherData.GetWeatherDescription()}");
                return weatherData;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener clima: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene clima usando nombre de ciudad/país (usa geocoding).
        /// </summary>
        public async Task<WeatherConditionData?> GetWeatherByCityAsync(string city, string countryCode)
        {
            try
            {
                _logger.LogInformation($"Obteniendo coordenadas para: {city}, {countryCode}");

                // Paso 1: Geocodificar la ubicación
                string geocodeUrl = $"{GeocodeUrl}/search?name={Uri.EscapeDataString(city)}&country_code={countryCode}&count=1&language=es&format=json";

                var geoResponse = await _httpClient.GetAsync(geocodeUrl);
                geoResponse.EnsureSuccessStatusCode();

                using var geoDoc = System.Text.Json.JsonDocument.Parse(await geoResponse.Content.ReadAsStringAsync());
                var geoRoot = geoDoc.RootElement;

                // Extraer primera coincidencia
                if (!geoRoot.GetProperty("results").EnumerateArray().Any())
                {
                    _logger.LogWarning($"No se encontró ubicación: {city}, {countryCode}");
                    return null;
                }

                var firstResult = geoRoot.GetProperty("results").EnumerateArray().First();
                double latitude = firstResult.GetProperty("latitude").GetDouble();
                double longitude = firstResult.GetProperty("longitude").GetDouble();

                // Paso 2: Obtener clima para esas coordenadas
                return await GetCurrentWeatherAsync(latitude, longitude);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error en GetWeatherByCityAsync: {ex.Message}");
                return null;
            }
        }
    }
}
