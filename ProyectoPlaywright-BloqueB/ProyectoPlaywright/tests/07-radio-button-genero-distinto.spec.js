// @ts-check
const { test, expect } = require('@playwright/test');
const { FormularioPage } = require('../pages/FormularioPage');
const { datosValidosBase } = require('../utils/helpers');

/**
 * CASO DE PRUEBA 7: Radio Button distinto (Género)
 * --------------------------------------------------
 * Objetivo: confirmar que el formulario acepta CUALQUIERA de las 3
 * opciones de género, y que seleccionar una desmarca las otras
 * automáticamente (comportamiento nativo de <input type="radio">).
 */
test.describe('Caso 7: Radio Button distinto (Género)', () => {

    for (const genero of ['Masculino', 'Femenino', 'Otro']) {
        test(`Debe registrar correctamente con género = ${genero}`, async ({ page }) => {
            const formulario = new FormularioPage(page);
            const datos = datosValidosBase({ genero });

            await formulario.ir();
            await formulario.llenarFormularioCompleto(datos);

            // Verifica que SOLO el radio seleccionado está marcado
            await expect(formulario.radioGenero(genero)).toBeChecked();

            const otrasOpciones = ['Masculino', 'Femenino', 'Otro'].filter(g => g !== genero);
            for (const otra of otrasOpciones) {
                await expect(formulario.radioGenero(otra)).not.toBeChecked();
            }

            await formulario.enviar();
            await page.waitForURL(/\/Registro\/Confirmacion\/\d+/, { timeout: 10000 });
            await expect(formulario.panelConfirmacion).toBeVisible();
        });
    }

});
