using Microsoft.EntityFrameworkCore;
using CoreWCF;
using CoreWCF.Configuration;
using FormularioGamerWeb.Data;
using FormularioGamerWeb.Contracts.SOAP.ServiceContracts;
using FormularioGamerWeb.Services.SOAP;
using FormularioGamerWeb.Services.REST;
using FormularioGamerWeb.Helpers;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. REGISTRO DE SERVICIOS (Inyección de Dependencias)
// ============================================================

// Agrega soporte para Controllers + Views (arquitectura MVC)
builder.Services.AddControllersWithViews();

// Registra el DbContext (Entity Framework Core) y le indica
// que use SQL Server con la cadena de conexión definida en appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================
// 2. REGISTRO DE SERVICIOS SOAP (CoreWCF)
// ============================================================

// Registra el servicio SOAP y sus dependencias
builder.Services.AddScoped<PerformanceCalculator>();
builder.Services.AddScoped<IPlayerPerformanceService, PlayerPerformanceService>();

// ============================================================
// 3. REGISTRO DE SERVICIOS REST (HTTP Client)
// ============================================================

// Registra HttpClient para consumir APIs externas
builder.Services.AddHttpClient<IWeatherClient, WeatherClient>();

builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata(); // ← agregar esta también

var app = builder.Build();

// ============================================================
// 3. CONFIGURAR SERVICIO SOAP (CoreWCF) - ANTES del pipeline
// ============================================================

app.UseServiceModel(serviceBuilder =>
{
    serviceBuilder.AddService<PlayerPerformanceService>(options =>
    {
        options.DebugBehavior.IncludeExceptionDetailInFaults = true;
    });

    serviceBuilder.AddServiceEndpoint<PlayerPerformanceService, IPlayerPerformanceService>(
        new BasicHttpBinding(), "/PlayerPerformanceService.svc");

    var metadata = app.Services.GetRequiredService<CoreWCF.Description.ServiceMetadataBehavior>();
    metadata.HttpGetEnabled = true;
});

// ============================================================
// 3. PIPELINE DE MIDDLEWARE (Orden de ejecución de cada request)
// ============================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseHttpsRedirection();
}

// Nota: en Development NO forzamos HTTPS. Esto evita problemas de
// certificados autofirmados al automatizar con Playwright en local.
// La app corre en http://localhost:5180 (ver Properties/launchSettings.json)

app.UseStaticFiles(); // Permite servir archivos de wwwroot (css, js, imágenes subidas)

app.UseRouting();

app.UseAuthorization();

// Ruta por defecto: si no se especifica nada, abre el Registro (nuestro formulario)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Registro}/{action=Index}/{id?}");

app.MapGet("/servicio", async context =>
{
    context.Response.Redirect("/Home/Servicio");
});

app.Run();