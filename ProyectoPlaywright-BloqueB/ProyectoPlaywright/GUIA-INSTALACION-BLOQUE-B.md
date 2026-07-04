# 🎮 GUÍA DE INSTALACIÓN Y USO — Proyecto Playwright (Bloque B)

## Requisitos previos

| Requisito | Verificar con | Dónde conseguirlo |
|---|---|---|
| Node.js 18 o superior | `node --version` | https://nodejs.org (botón "LTS") |
| VS Code | Abrir VS Code | https://code.visualstudio.com |
| App ASP.NET corriendo | Abrir http://localhost:5180 | Ejecutar Bloque A con F5 en Visual Studio |

---

## PASO 1 — Abrir la carpeta en VS Code

1. Abre **VS Code**.
2. Menú **File → Open Folder**.
3. Navega hasta la carpeta `ProyectoPlaywright` y haz clic en **Seleccionar carpeta**.
4. VS Code muestra el árbol de archivos en el panel izquierdo.

---

## PASO 2 — Abrir la terminal integrada

Presiona `` Ctrl + ` `` (tecla de acento grave, debajo de `Esc`).
Aparece una terminal en la parte inferior de VS Code.
Confirma que estás dentro de la carpeta correcta:

```
ProyectoPlaywright>
```

Si no, navega con: `cd ruta\a\tu\carpeta\ProyectoPlaywright`

---

## PASO 3 — Instalar dependencias Node.js

En la terminal escribe exactamente:

```bash
npm install
```

Esto lee el archivo `package.json` y descarga todas las librerías
necesarias (`playwright`, `xlsx`, `dotenv`) dentro de la carpeta
`node_modules/`. Puede tardar 30–60 segundos.

Cuando termine verás algo así:
```
added 87 packages in 25s
```

---

## PASO 4 — Descargar los navegadores de Playwright

Playwright no usa el navegador que tienes instalado en Windows.
Descarga sus propias versiones portátiles de Chromium, Firefox y WebKit:

```bash
npx playwright install
```

> ⚠️ Descarga ~300 MB. Solo se hace una vez por equipo.

Cuando termine verás:
```
✅ Chromium 128.0 (playwright build) downloaded
✅ Firefox 129.0 downloaded
✅ WebKit 18.0 downloaded
```

---

## PASO 5 — Instalar la extensión oficial de Playwright en VS Code

1. Haz clic en el ícono de **Extensiones** en la barra lateral izquierda
   (parece cuatro cuadros, o usa `Ctrl + Shift + X`).
2. Busca: `Playwright Test for VSCode`
3. El autor debe ser **Microsoft**.
4. Haz clic en **Install**.

Esta extensión añade un panel "Testing" con botones para correr
cada test individualmente con un clic.

---

## PASO 6 — Iniciar la aplicación ASP.NET (Bloque A)

**La app web DEBE estar corriendo ANTES de ejecutar cualquier test.**

1. Abre Visual Studio 2022 con el proyecto `FormularioGamerWeb`.
2. Presiona **F5** (o el botón verde ▶).
3. Verifica que el navegador abra `http://localhost:5180/Registro/Index`
   y el formulario sea visible.
4. **Deja Visual Studio abierto** y vuelve a VS Code.

---

## PASO 7 — Generar el Excel (si no tienes el archivo ya)

El archivo `data/registros.xlsx` ya viene incluido con 55 registros.
Si necesitas regenerarlo o cambiar la cantidad:

```bash
node data/generate-excel-data.js
```

Para generar una cantidad específica (ej: 100):

```bash
node data/generate-excel-data.js 100
```

---

## PASO 8 — Ejecutar los tests

### Opción A: Con la extensión de VS Code (recomendado para aprender)

1. Haz clic en el ícono de **Testing** en la barra lateral izquierda
   (parece un matraz de laboratorio o un triángulo).
2. Verás la lista de todos los tests agrupados por archivo.
3. Haz clic en ▶ junto a cualquier test para ejecutarlo.
4. Los resultados aparecen con ✅ (pasó) o ❌ (falló).

### Opción B: Desde la terminal

```bash
# Ejecutar TODOS los tests (modo headless, sin abrir navegador)
npm test

# Ver el navegador mientras se ejecutan
npm run test:headed

# Ejecutar solo un archivo específico
npx playwright test 01-registro-completo-valido.spec.js --headed

# Ejecutar con pausa para depurar paso a paso
npm run test:debug

# Ver reporte HTML de la última ejecución
npm run test:report
```

### Comandos especiales (modos Excel)

```bash
# Solo el primer registro del Excel
npm run test:un-registro

# Todos los 55 registros del Excel
npm run test:todos-los-registros
```

---

## PASO 9 — Verificar que los datos llegaron a SQL Server

### Opción A: En el navegador (más rápido)

```
http://localhost:5180/Registro/Lista
```
Muestra una tabla con todos los jugadores registrados.

```
http://localhost:5180/Registro/ApiUsuarios
```
Devuelve un JSON con todos los registros.

### Opción B: En SQL Server Management Studio (SSMS)

```sql
-- Ver todos los registros ordenados por fecha
SELECT * FROM RegistrosJugadores ORDER BY FechaRegistro DESC;

-- Contar cuántos se registraron
SELECT COUNT(*) AS Total FROM RegistrosJugadores;

-- Ver los últimos 10 (después de correr el modo todos-los-registros)
SELECT TOP 10 Nombre, Apellido, Email, Pais, FechaRegistro
FROM RegistrosJugadores
ORDER BY FechaRegistro DESC;

-- Limpiar todos los registros para volver a probar desde cero
TRUNCATE TABLE RegistrosJugadores;
```

---

## Estructura del proyecto

```
ProyectoPlaywright/
│
├── 📄 playwright.config.js        Configuración central de Playwright
├── 📄 package.json                Dependencias y scripts npm
├── 📄 .env                        Variables de entorno (URL de la app, etc.)
│
├── 📂 tests/                      Archivos de prueba (uno por caso)
│   ├── 01-registro-completo-valido.spec.js     waitForURL
│   ├── 02-campo-obligatorio-vacio.spec.js      waitForSelector
│   ├── 03-correo-invalido.spec.js              Locator.waitFor
│   ├── 04-edad-fuera-de-rango.spec.js          waitForLoadState
│   ├── 05-archivo-incorrecto.spec.js           Auto-Waiting
│   ├── 06-checkbox-terminos-sin-seleccionar.spec.js
│   ├── 07-radio-button-genero-distinto.spec.js
│   ├── 08-fecha-invalida.spec.js               waitForTimeout
│   ├── 09-valores-extremos-slider.spec.js      waitForFunction
│   ├── 10-validacion-en-base-de-datos.spec.js  waitForResponse
│   ├── 11-un-registro-excel.spec.js            Modo: 1 registro
│   └── 12-todos-los-registros-excel.spec.js    Modo: todos los registros
│
├── 📂 pages/
│   └── FormularioPage.js          Page Object Model (todos los selectores)
│
├── 📂 utils/
│   ├── excelReader.js             Lee y normaliza el archivo Excel
│   └── helpers.js                 Funciones reutilizables (emails únicos, etc.)
│
└── 📂 data/
    ├── registros.xlsx             Excel con 55 jugadores de prueba
    ├── generate-excel-data.js     Generador del Excel
    └── images/
        ├── avatar-sample.png      Imagen válida para el campo Avatar
        └── archivo-invalido.txt   Archivo inválido para el Caso 5
```

---

## Resumen de estrategias de espera (Wait)

| Test | Estrategia | Cuándo usarla |
|---|---|---|
| 01 | `waitForURL()` | Después de un submit que redirige a otra URL |
| 02 | `waitForSelector()` | Esperar a que un selector específico aparezca en el DOM |
| 03 | `Locator.waitFor()` | Igual que waitForSelector pero sobre un locator ya creado |
| 04 | `waitForLoadState()` | Esperar a que la página termine de cargar completamente |
| 05 | Auto-Waiting | Playwright espera automáticamente antes de cada acción |
| 08 | `waitForTimeout()` | Pausa fija; solo cuando hay animaciones o demos |
| 09 | `waitForFunction()` | Condición arbitraria en JavaScript del navegador |
| 10 | `waitForResponse()` | Esperar una respuesta HTTP específica del servidor |

---

## Solución de errores comunes

| Error | Causa | Solución |
|---|---|---|
| `Error: connect ECONNREFUSED localhost:5180` | La app ASP.NET no está corriendo | Abrir Visual Studio y presionar F5 |
| `Timeout waiting for selector` | El formulario tardó más de lo esperado | Aumentar `actionTimeout` en `playwright.config.js` |
| `El correo ya está registrado` | El test se ejecutó antes con el mismo email | Los emails son únicos por timestamp, debería no ocurrir. Si pasa: `TRUNCATE TABLE RegistrosJugadores;` en SSMS |
| `Cannot find module '../pages/FormularioPage'` | `npm install` no se ejecutó | Correr `npm install` en la terminal |
| `browserType.launch: Executable doesn't exist` | Los navegadores no están descargados | Correr `npx playwright install` |

---

## Flujo completo en orden

```
1. Visual Studio: F5 → La app corre en http://localhost:5180
2. VS Code terminal: npm install
3. VS Code terminal: npx playwright install  (solo la primera vez)
4. VS Code terminal: npm run test:headed     (ver todos los tests)
5. SSMS: SELECT * FROM RegistrosJugadores;  (verificar datos)
6. Navegador: http://localhost:5180/Registro/Lista  (verificar visualmente)
```
