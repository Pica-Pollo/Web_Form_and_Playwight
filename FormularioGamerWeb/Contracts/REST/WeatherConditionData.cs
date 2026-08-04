using System.Net.Http.Json;

namespace FormularioGamerWeb.Contracts.REST
{
    /// <summary>
    /// Respuesta de la API REST externa de clima.
    /// Usamos Open-Meteo Weather API (gratuita, sin key, estable).
    /// 
    /// Endpoint: https://api.open-meteo.com/v1/forecast
    /// Documentación: https://open-meteo.com/en/docs
    /// </summary>
    public class WeatherConditionData
    {
        public class LocationCoordinates
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string Timezone { get; set; } = string.Empty;
        }

        public class CurrentWeather
        {
            public double Temperature { get; set; }
            public double WindSpeed { get; set; }
            public int WeatherCode { get; set; }
        }

        public LocationCoordinates? Coordinates { get; set; }
        public CurrentWeather? Current { get; set; }

        public string GetWeatherDescription()
        {
            if (Current?.WeatherCode == null)
                return "Desconocido";

            return Current.WeatherCode switch
            {
                0 => "Cielo despejado",
                1 or 2 => "Mayormente despejado",
                3 => "Parcialmente nublado",
                45 or 48 => "Niebla",
                51 or 53 or 55 => "Llovizna",
                61 or 63 or 65 => "Lluvia",
                71 or 73 or 75 => "Nieve",
                77 => "Nieve granulada",
                80 or 82 or 85 => "Lluvia con nieve",
                95 or 96 or 99 => "Tormenta",
                _ => "Condición desconocida"
            };
        }
    }
}
