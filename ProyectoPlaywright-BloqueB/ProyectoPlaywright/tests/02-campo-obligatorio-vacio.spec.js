// @ts-check
const { test, expect } = require('@playwright/test');
const { FormularioPage } = require('../pages/FormularioPage');
const { datosValidosBase } = require('../utils/helpers');

/**
 * CASO DE PRUEBA 2: Campo obligatorio vacío
 * --------------------------------------------------
 * Objetivo: confirmar que el formulario RECHAZA el envío si se deja
 * el campo "Nombre" vacío (es obligatorio según [Required] en el Modelo).
 *
 * ESTRATEGIA DE ESPERA DEMOSTRADA: page.waitForSelector()
 * ¿Por qué? Después de un submit fallido, el servidor vuelve a renderizar
 * la misma vista con la alerta de error. waitForSelector() espera
 * explícitamente a que ese selector exista en el DOM antes de continuar,
 * evitando falsos negativos si la respuesta del servidor tarda un poco.
 */
test.describe('Caso 2: Campo obligatorio vacío', () => {

    test('Debe rechazar el envío si el Nombre está vacío', async ({ page }) => {
        const formulario = new FormularioPage(page);
        const datos = datosValidosBase({ nombre: '' }); // Nombre vacío a propósito

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos);
        await formulario.enviar();

        // ESPERA EXPLÍCITA POR SELECTOR: espera a que aparezca el bloque
        // de errores de validación en el DOM
        await page.waitForSelector('[data-testid="alerta-validacion"]', { timeout: 8000 });

        await expect(formulario.alertaValidacion).toBeVisible();

        // La URL NO debe haber cambiado a Confirmacion
        await expect(page).not.toHaveURL(/Confirmacion/);

        console.log('✅ El formulario rechazó correctamente el campo Nombre vacío');
    });

});
