// @ts-check
const { test, expect } = require('@playwright/test');
const path = require('path');
const { FormularioPage } = require('../pages/FormularioPage');
const { datosValidosBase } = require('../utils/helpers');

/**
 * CASO DE PRUEBA 5: Archivo incorrecto en el Avatar
 * --------------------------------------------------
 * Objetivo: el controlador (RegistroController) valida que el archivo
 * subido tenga extensión de imagen (.jpg, .png, .gif, .webp). Subimos
 * un archivo .txt y confirmamos que el servidor lo rechaza.
 *
 * ESTRATEGIA DE ESPERA DEMOSTRADA: Auto-Waiting (esperas automáticas)
 * ¿Por qué? Playwright espera AUTOMÁTICAMENTE (sin que escribamos código
 * de espera) a que cada elemento esté "accionable" (visible, habilitado,
 * estable, recibiendo eventos) antes de actuar sobre él. En este test NO
 * agregamos ningún waitFor/waitForSelector manual: cada .fill(), .check()
 * y .click() ya espera por sí mismo. Esto es la base de TODOS los tests
 * de este proyecto; aquí simplemente lo dejamos explícito y comentado.
 */
test.describe('Caso 5: Archivo incorrecto', () => {

    test('Debe rechazar un archivo que no es una imagen válida', async ({ page }) => {
        const formulario = new FormularioPage(page);
        const datos = datosValidosBase();
        const rutaArchivoInvalido = path.resolve(__dirname, '../data/images/archivo-invalido.txt');

        await formulario.ir();

        // Auto-Waiting en acción: Playwright espera a que cada input
        // esté listo antes de interactuar, sin código adicional de espera.
        await formulario.llenarFormularioCompleto(datos, null);
        await formulario.inputAvatar.setInputFiles(rutaArchivoInvalido);

        await formulario.enviar();

        // expect().toBeVisible() también incorpora auto-espera internamente:
        // reintenta la aserción hasta el timeout en vez de fallar de inmediato.
        await expect(formulario.alertaValidacion).toBeVisible();
        await expect(page).not.toHaveURL(/Confirmacion/);
    });

});
