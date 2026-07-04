// @ts-check
const { test, expect } = require('@playwright/test');
const { FormularioPage } = require('../pages/FormularioPage');
const { datosValidosBase } = require('../utils/helpers');

/**
 * CASO DE PRUEBA 3: Correo electrónico inválido
 * --------------------------------------------------
 * Objetivo: confirmar que el formulario rechaza un email con formato
 * incorrecto (validado por [EmailAddress] en el Modelo).
 *
 * ESTRATEGIA DE ESPERA DEMOSTRADA: Locator.waitFor()
 * ¿Por qué? A diferencia de page.waitForSelector() (que busca en toda
 * la página), Locator.waitFor() se invoca sobre un locator YA definido
 * en el Page Object (this.errorEmail), lo que hace el código más legible
 * y reutilizable. Esperamos a que ESE locator específico esté visible.
 */
test.describe('Caso 3: Correo inválido', () => {

    test('Debe rechazar un correo con formato incorrecto', async ({ page }) => {
        const formulario = new FormularioPage(page);
        const datos = datosValidosBase({ email: 'esto-no-es-un-correo' });

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos);
        await formulario.enviar();

        // ESPERA EXPLÍCITA SOBRE UN LOCATOR: Locator.waitFor()
        await formulario.errorEmail.waitFor({ state: 'visible', timeout: 8000 });

        await expect(formulario.errorEmail).toContainText(/correo|formato/i);
        await expect(page).not.toHaveURL(/Confirmacion/);

        console.log('✅ El formulario rechazó correctamente el correo inválido');
    });

});
