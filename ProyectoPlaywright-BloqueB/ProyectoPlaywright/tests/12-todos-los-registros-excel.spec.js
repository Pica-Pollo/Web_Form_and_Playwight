// @ts-check
const { test, expect } = require('@playwright/test');
const path = require('path');
const { FormularioPage } = require('../pages/FormularioPage');
const { leerRegistrosExcel } = require('../utils/excelReader');
const { generarEmailUnico } = require('../utils/helpers');

/**
 * MODO 2: TODOS LOS REGISTROS DESDE EXCEL
 * --------------------------------------------------
 * Itera CADA FILA del Excel y la ingresa en el formulario web
 * una por una. Al terminar, todos los jugadores estarán
 * registrados en SQL Server.
 *
 * Cómo ejecutar SOLO este archivo:
 *   npx playwright test 12-todos-los-registros-excel.spec.js --headed
 *   o:
 *   npm run test:todos-los-registros
 *
 * Nota de tiempo: con 50 registros y 1-2 segundos por registro,
 * la ejecución completa toma ~2 minutos. Para seguirla visualmente
 * usa --headed.
 *
 * SELECTORES ADICIONALES demostrados en este test:
 *   - first-child, nth-child, last-child (ya en FormularioPage.js)
 *   - Selector compuesto (clase + atributo): .form-section[data-section="datos-basicos"] input
 *
 * ESTRATEGIAS DE ESPERA en este test:
 *   - waitForSelector() al inicio de cada iteración (verifica que el
 *     formulario cargó antes de empezar a llenarlo).
 *   - waitForURL() al final de cada iteración (confirma la redirección).
 *   - Auto-Waiting implícito en cada fill(), click(), selectOption().
 */
test.describe('Modo 2: Todos los registros desde Excel', () => {

    test.slow();

    // beforeAll: leemos el Excel UNA SOLA VEZ antes de todos los tests
    let registros = [];
    const rutaExcel = path.resolve(__dirname, '../data/registros.xlsx');
    const rutaImagen = path.resolve(__dirname, '../data/images/avatar-sample.png');

    test.beforeAll(async () => {
        registros = leerRegistrosExcel(rutaExcel);
        console.log(`\n📊 MODO 2 — Todos los registros desde Excel`);
        console.log(`   Total de registros a procesar: ${registros.length}`);
        console.log(`   Archivo Excel: ${rutaExcel}\n`);
        expect(registros.length).toBeGreaterThan(0);
    });

    // Generamos un test dinámico por cada fila del Excel.
    // Playwright los ejecutará secuencialmente (fullyParallel: false en config).
    test('Debe procesar e ingresar todos los registros del Excel', async ({ page }) => {
        const formulario = new FormularioPage(page);

        let exitosos = 0;
        let fallidos = 0;
        const errores = [];

        for (let i = 0; i < registros.length; i++) {
            const registro = { ...registros[i] };
            // Email único por iteración para evitar colisiones en re-ejecuciones
            registro.email = generarEmailUnico(`${registro.nombre}${i + 1}`);

            console.log(`\n[${i + 1}/${registros.length}] Procesando: ${registro.nombre} ${registro.apellido}`);

            try {
                // ── Navegar al formulario ──────────────────────────────────
                await formulario.ir();

                // waitForSelector: verificar que el campo Nombre existe y está listo
                // ANTES de empezar a llenar el formulario de esta iteración.
                await page.waitForSelector('#Nombre', {
                    state: 'visible',
                    timeout: 10000
                });

                // ── Llenar todos los controles ─────────────────────────────
                await formulario.llenarFormularioCompleto(registro, rutaImagen);

                // ── Enviar ─────────────────────────────────────────────────
                await formulario.enviar();

                // waitForURL: confirmar redirección a Confirmacion
                await page.waitForURL(/\/Registro\/Confirmacion\/\d+/, { timeout: 12000 });
                await expect(formulario.panelConfirmacion).toBeVisible();

                const id = await formulario.resumenId.textContent();
                console.log(`   ✅ Registrado. ID en BD: ${id}`);
                exitosos++;

            } catch (error) {
                console.error(`   ❌ Error en registro ${i + 1}: ${error.message}`);
                fallidos++;
                errores.push({ fila: i + 1, nombre: registro.nombre, error: error.message });
                // Continuar con el siguiente registro aunque este falle
                continue;
            }
        }

        // ── Resumen final ──────────────────────────────────────────────────
        console.log('\n' + '='.repeat(55));
        console.log('📊 RESUMEN DE LA EJECUCIÓN');
        console.log('='.repeat(55));
        console.log(`   Total procesados : ${registros.length}`);
        console.log(`   ✅ Exitosos       : ${exitosos}`);
        console.log(`   ❌ Fallidos       : ${fallidos}`);
        if (errores.length > 0) {
            console.log('\n   Detalle de errores:');
            errores.forEach(e => console.log(`     Fila ${e.fila} (${e.nombre}): ${e.error}`));
        }
        console.log('='.repeat(55));
        console.log('\n   Verificar en SSMS:');
        console.log('   SELECT * FROM RegistrosJugadores ORDER BY FechaRegistro DESC;');
        console.log('\n   O en el navegador:');
        console.log('   http://localhost:5180/Registro/Lista');

        // El test falla si NINGÚN registro se procesó
        expect(exitosos).toBeGreaterThan(0);
    });

});
