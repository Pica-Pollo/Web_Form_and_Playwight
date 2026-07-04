// @ts-check
const { test, expect } = require('@playwright/test');
const path = require('path');
const { FormularioPage } = require('../pages/FormularioPage');
const { leerRegistrosExcel } = require('../utils/excelReader');
const { generarEmailUnico } = require('../utils/helpers');

/**
 * MODO 1: UN SOLO REGISTRO DESDE EXCEL
 * --------------------------------------------------
 * Ejecuta este archivo para ingresar ÚNICAMENTE el primer registro
 * del Excel. Útil para:
 *   - Verificar rápidamente que la conexión formulario → BD funciona.
 *   - Probar un registro específico sin esperar a que procesen todos.
 *   - Demostrar el flujo completo en una sola ejecución.
 *
 * Cómo ejecutar SOLO este archivo:
 *   npx playwright test 11-un-registro-excel.spec.js --headed
 *   o:
 *   npm run test:un-registro
 *
 * SELECTORES usados en este test (distintos a los de tests anteriores):
 *   - Selector por placeholder:  input[placeholder="Akira"] -> no, usamos
 *     el Page Object (que ya tiene selectores variados).
 *   - Selectores del POM: ver FormularioPage.js para el catálogo completo.
 *
 * ESTRATEGIA DE ESPERA:
 *   Combina waitForLoadState + waitForURL, demostrando que se pueden
 *   encadenar varias estrategias de espera en el mismo test.
 */
test.describe('Modo 1: Un registro desde Excel', () => {

    test('Debe registrar el primer jugador del Excel en la BD', async ({ page }) => {
        // ── 1. Leer Excel ────────────────────────────────────────────────
        const rutaExcel = path.resolve(__dirname, '../data/registros.xlsx');
        const registros = leerRegistrosExcel(rutaExcel);

        expect(registros.length).toBeGreaterThan(0);
        const primerRegistro = { ...registros[0] };

        // Hacemos el email único para no colisionar si se re-ejecuta el test
        primerRegistro.email = generarEmailUnico(`${primerRegistro.nombre}.${primerRegistro.apellido}`);

        console.log('\n📊 MODO 1 — Un registro desde Excel');
        console.log(`   Procesando: ${primerRegistro.nombre} ${primerRegistro.apellido}`);
        console.log(`   Email único: ${primerRegistro.email}`);

        // ── 2. Navegar al formulario ─────────────────────────────────────
        const formulario = new FormularioPage(page);
        await formulario.ir();

        // waitForLoadState: asegurar que la página y sus recursos están listos
        await page.waitForLoadState('domcontentloaded');

        // ── 3. Llenar todos los controles con los datos del Excel ─────────
        const rutaImagen = path.resolve(__dirname, '../data/images/avatar-sample.png');
        await formulario.llenarFormularioCompleto(primerRegistro, rutaImagen);

        // ── 4. Enviar el formulario ───────────────────────────────────────
        await formulario.enviar();

        // waitForURL: espera la redirección a la pantalla de confirmación
        await page.waitForURL(/\/Registro\/Confirmacion\/\d+/, { timeout: 15000 });

        // ── 5. Verificar confirmación ─────────────────────────────────────
        await expect(formulario.panelConfirmacion).toBeVisible();
        const idAsignado = await formulario.resumenId.textContent();

        console.log(`✅ Registro completado. ID en BD: ${idAsignado}`);
        console.log(`   Nombre: ${primerRegistro.nombre} ${primerRegistro.apellido}`);
    });

});
