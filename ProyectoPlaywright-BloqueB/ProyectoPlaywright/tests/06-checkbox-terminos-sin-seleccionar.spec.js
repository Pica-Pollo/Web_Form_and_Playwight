// @ts-check
const { test, expect } = require('@playwright/test');
const { FormularioPage } = require('../pages/FormularioPage');
const { datosValidosBase } = require('../utils/helpers');

/**
 * CASO DE PRUEBA 6: Checkbox de Términos y Condiciones sin marcar
 * --------------------------------------------------
 * Objetivo: el controlador valida explícitamente que AceptaTerminos
 * sea true antes de guardar. Si el checkbox no se marca, debe rechazar.
 */
test.describe('Caso 6: Checkbox de términos sin seleccionar', () => {

    test('Debe rechazar el envío si no se aceptan los términos', async ({ page }) => {
        const formulario = new FormularioPage(page);
        // acepta_terminos = 'NO' -> llenarFormularioCompleto NO marcará el checkbox
        const datos = datosValidosBase({ acepta_terminos: 'NO' });

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos);

        await expect(formulario.checkboxTerminos).not.toBeChecked();

        await formulario.enviar();

        await formulario.errorTerminos.waitFor({ state: 'visible', timeout: 8000 });
        await expect(formulario.errorTerminos).toBeVisible();
        await expect(page).not.toHaveURL(/Confirmacion/);
    });

});
