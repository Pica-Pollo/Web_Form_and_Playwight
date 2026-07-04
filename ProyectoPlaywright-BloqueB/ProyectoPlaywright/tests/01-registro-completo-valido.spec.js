// @ts-check
const { test, expect } = require('@playwright/test');
const { FormularioPage } = require('../pages/FormularioPage');
const { datosValidosBase } = require('../utils/helpers');

/**
 * CASO DE PRUEBA 1: Registro completamente válido
 * --------------------------------------------------
 * Objetivo: confirmar que, cuando todos los campos son válidos,
 * el formulario se envía correctamente y el usuario es redirigido
 * a la página de confirmación.
 *
 * ESTRATEGIA DE ESPERA DEMOSTRADA: page.waitForURL()
 * ¿Por qué? Tras el submit, el servidor hace un Redirect (Post-Redirect-Get)
 * hacia /Registro/Confirmacion/{id}. waitForURL() espera explícitamente a
 * que la URL del navegador cambie al patrón esperado, lo cual es más
 * confiable que esperar un tiempo fijo, porque no sabemos de antemano
 * cuánto tardará el servidor en procesar y redirigir.
 */
test.describe('Caso 1: Registro válido', () => {

    test('Debe registrar un jugador con todos los datos válidos', async ({ page }) => {
        const formulario = new FormularioPage(page);
        const datos = datosValidosBase();

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos);
        await formulario.enviar();

        // ESPERA EXPLÍCITA POR URL: espera a que el navegador navegue
        // a una URL que coincida con /Registro/Confirmacion/<numero>
        await page.waitForURL(/\/Registro\/Confirmacion\/\d+/, { 
            waitUntil: 'domcontentloaded', 
            timeout: 10000 
        });

        await expect(formulario.panelConfirmacion).toBeVisible();
        await expect(formulario.resumenEmail).toHaveText(datos.email);

        console.log(`✅ Registro creado para: ${datos.email}`);
    });

});
