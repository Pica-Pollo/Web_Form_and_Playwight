// @ts-check
const { test, expect } = require('@playwright/test');
const { FormularioPage } = require('../pages/FormularioPage');
const { datosValidosBase } = require('../utils/helpers');

/**
 * CASO DE PRUEBA 8: Fecha de nacimiento inválida
 * --------------------------------------------------
 * Objetivo: verificar que el formulario rechaza:
 *   a) Una fecha futura (aún no nacido).
 *   b) Una fecha de hace más de 120 años (imposible).
 *
 * ESTRATEGIA DE ESPERA DEMOSTRADA: page.waitForTimeout()
 * -------------------------------------------------------
 * waitForTimeout() es una PAUSA FIJA en milisegundos. Playwright
 * desaconseja usarla como estrategia principal porque hace los tests
 * más lentos y frágiles (si el servidor tarda más que el tiempo fijo,
 * el test falla igual).
 *
 * ¿Cuándo SÍ tiene sentido?
 *   - Después de acciones que disparan animaciones CSS (el elemento
 *     ya es visible en el DOM pero todavía está animándose y no es
 *     "estable" para interactuar).
 *   - Para dar tiempo a que una alerta JavaScript (alert/confirm) se
 *     procese antes de hacer otra acción.
 *   - En demos o grabaciones donde queremos que el tester vea cada
 *     paso lentamente.
 *
 * En este test la usamos DESPUÉS de hacer submit, para dar tiempo a
 * que la animación de la alerta de error termine de renderizarse
 * antes de inspeccionarla. Luego combinamos con una aserción normal.
 */
test.describe('Caso 8: Fecha de nacimiento inválida', () => {

    test('Debe rechazar una fecha de nacimiento futura (año 2090)', async ({ page }) => {
        const formulario = new FormularioPage(page);
        // Fecha en el futuro -> persona que todavía no nació
        const datos = datosValidosBase({ fecha_nacimiento: '2090-01-01', edad: 18 });

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos);
        await formulario.enviar();

        // waitForTimeout: espera fija de 800ms para que la animación
        // de la alerta de error termine de renderizarse en el DOM.
        // Nota: en el 99% de los casos, las esperas de Playwright
        // (Auto-Waiting) son suficientes. Esto es un ejemplo didáctico.
        await expect(formulario.alertaValidacion).toBeVisible();

        await expect(formulario.alertaValidacion).toBeVisible();
        await expect(page).not.toHaveURL(/Confirmacion/);

        console.log('✅ Rechazó correctamente la fecha futura 2090-01-01');
    });

    test('Debe rechazar una fecha de hace más de 120 años (año 1890)', async ({ page }) => {
        const formulario = new FormularioPage(page);
        const datos = datosValidosBase({ fecha_nacimiento: '1890-06-15', edad: 18 });

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos);
        await formulario.enviar();

        // Misma pausa fija para demostrar la estrategia en un segundo sub-test
        await expect(formulario.alertaValidacion).toBeVisible();

        await expect(formulario.alertaValidacion).toBeVisible();
        await expect(page).not.toHaveURL(/Confirmacion/);

        console.log('✅ Rechazó correctamente la fecha 1890-06-15');
    });

});
