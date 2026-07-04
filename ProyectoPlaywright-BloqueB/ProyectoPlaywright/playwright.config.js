// @ts-check
const { defineConfig, devices } = require('@playwright/test');
require('dotenv').config();

/**
 * playwright.config.js
 *
 * Configuración central de Playwright para este proyecto.
 * IMPORTANTE: la aplicación ASP.NET Core (Bloque A) debe estar
 * corriendo en http://localhost:5180 ANTES de ejecutar los tests
 * (inícialo desde Visual Studio con F5).
 *
 * Este proyecto Playwright es completamente independiente de la
 * aplicación web: viven en carpetas/repos separados, tal como
 * se pidió en las especificaciones.
 */
module.exports = defineConfig({
  testDir: './tests',

  // Tiempo máximo por test individual
  timeout: 60 * 1000,

  // Ejecuta los archivos de test en paralelo (cada uno en su propio worker)
  fullyParallel: false, // false: porque varios tests escriben en la misma BD (emails únicos por test, pero más predecible en serie para aprender)

  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  workers: process.env.CI ? 1 : undefined,

  // Reporte HTML navegable (se abre con: npm run test:report)
  reporter: [
    ['html', { open: 'never' }],
    ['list']
  ],

  use: {
    baseURL: process.env.BASE_URL || 'http://localhost:5180',

    // Screenshots y video solo cuando un test falla (ahorra espacio)
    screenshot: 'only-on-failure',
    video: 'retain-on-failure',
    trace: 'on-first-retry',

    // Espera automática de Playwright (Auto-Waiting) antes de cada acción
    actionTimeout: 10 * 1000,
    navigationTimeout: 15 * 1000,
  },

  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
