using Microsoft.EntityFrameworkCore;
using FormularioGamerWeb.Data;

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

var app = builder.Build();

// ============================================================
// 2. PIPELINE DE MIDDLEWARE (Orden de ejecución de cada request)
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

app.Run();