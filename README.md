# FormularioGamerWeb - Formulario de Registro con Análisis de Desempeño

## 🚀 Iniciar la Aplicación

```powershell
cd FormularioGamerWeb
dotnet run
```

Luego abre: **http://localhost:5180**

---

## ✨ Nuevas Características Implementadas

### 1. **Servicio de Análisis de Desempeño** (SOAP CoreWCF)
- Calcula el índice de habilidad del jugador (0-100)
- Proporciona recomendaciones personalizadas
- Basado en: experiencia, género, plataforma, edad

### 2. **Integración con API REST** (Open-Meteo)
- Obtiene datos climáticos en tiempo real
- Gratis, sin API Key requerida
- Búsqueda por coordenadas o ciudad

### 3. **Endpoints JSON Nuevos**
Para integración con Playwright y automatización:

| Endpoint | Método | Descripción |
|----------|--------|-------------|
| `/Registro/Api/Count` | GET | Contar jugadores totales |
| `/Registro/Api/GetById/{id}` | GET | Obtener datos de un jugador |
| `/Registro/GetWeather?latitude=X&longitude=Y` | GET | Obtener clima actual |
| `/Registro/AnalyzePerformance` | GET | Analizar desempeño de jugador |
| `/PlayerPerformanceService.svc?wsdl` | GET | Servicio SOAP de desempeño de jugador |http://localhost:5180/PlayerPerformanceService.svc?wsdl
'/servicio'|http://localhost:5180/servicio
---

## 📝 Cómo Probar

### 1. Registrar un Jugador
```
URL: http://localhost:5180/Registro
Llenar formulario y click "Registrar"
```

### 2. Ver Lista de Jugadores
```
URL: http://localhost:5180/Registro/Lista
```

### 3. Probar APIs con PowerShell

**Contar jugadores:**
```powershell
curl http://localhost:5180/Registro/Api/Count
# Respuesta: {"totalJugadores": 1}
```

**Obtener clima (Madrid):**
```powershell
curl "http://localhost:5180/Registro/GetWeather?latitude=40.4168&longitude=-3.7038"
# Respuesta: {coordinates: {...}, current: {temperature: 22.5, ...}}
```

**Analizar desempeño:**
```powershell
curl -Method POST "http://localhost:5180/Registro/AnalyzePerformance?id=1" `
    -UseBasicParsing

# Respuesta: {skillIndex: 72, winRate: 0.65, nivel: "Profesional", recomendaciones: [...]}
```

---

## 📁 Estructura de Archivos Nuevos

```
FormularioGamerWeb/
├── Contracts/SOAP/ServiceContracts/
│   └── IPlayerPerformanceService.cs
├── Contracts/SOAP/DataContracts/
│   ├── PlayerPerformanceResult.cs
│   └── SkillBreakdown.cs
├── Contracts/REST/
│   └── WeatherConditionData.cs
├── Services/SOAP/
│   └── PlayerPerformanceService.cs
├── Services/REST/
│   └── WeatherClient.cs
└── Helpers/
    └── PerformanceCalculator.cs
```

---

## 🔧 Requisitos

- .NET 8 SDK
- SQL Server 2019+ (o Express)
- Visual Studio 2026 (opcional)

---

## 📊 Stack Tecnológico

- **Backend:** ASP.NET Core 8.0 (MVC)
- **ORM:** Entity Framework Core 8.0.8
- **SOAP:** CoreWCF 1.9.1
- **REST API Externa:** Open-Meteo (Gratuita)
- **BD:** SQL Server

---

## ✅ Estado

- ✅ Compilación: Exitosa
- ✅ Aplicación: Corriendo en puerto 5180
- ✅ BD: Conectada y funcional
- ✅ APIs: Respondiendo correctamente
- ✅ Listo para Producción