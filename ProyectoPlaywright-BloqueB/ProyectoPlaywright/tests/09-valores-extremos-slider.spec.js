// @ts-check
const { test, expect } = require('@playwright/test');
const { FormularioPage } = require('../pages/FormularioPage');
const { datosValidosBase } = require('../utils/helpers');

/**
 * CASO DE PRUEBA 9: Valores extremos del Range Slider
 * --------------------------------------------------
 * Objetivo: confirmar que el slider de "Nivel de Experiencia"
 * acepta y envía correctamente sus valores extremos: 1 (mínimo)
 * y 100 (máximo), y que el texto de la etiqueta se actualiza
 * en tiempo real al cambiar el valor.
 *
 * ESTRATEGIA DE ESPERA DEMOSTRADA: page.waitForFunction()
 * -------------------------------------------------------
 * waitForFunction() ejecuta un fragmento de JavaScript EN el
 * navegador y espera (re-intentando) hasta que devuelva un valor
 * "truthy" (verdadero). Es la única forma de esperar a que una
 * condición ARBITRARIA del DOM o del estado de la app sea verdadera,
 * cuando no existe un selector específico que aparezca o desaparezca.
 *
 * En este test la usamos para verificar que el texto del <output>
 * (que muestra el valor numérico del slider) se actualizó al valor
 * esperado ANTES de continuar con el envío. Esto evita enviar el
 * formulario con el valor antiguo si el evento 'input' del slider
 * tuviera alguna demora.
 */
test.describe('Caso 9: Valores extremos del slider de experiencia', () => {

    test('Debe aceptar y mostrar el valor mínimo del slider (1)', async ({ page }) => {
        const formulario = new FormularioPage(page);
        const datos = datosValidosBase({ nivel_experiencia: 1 });

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos);

        // waitForFunction: espera hasta que el <output> muestre "1"
        // Pasamos el valor esperado como argumento (buena práctica:
        // no evaluar variables de Node.js directamente dentro del
        // string del browser context).
        await page.waitForFunction(
            (valorEsperado) => {
                const output = document.getElementById('valorExperiencia');
                return output !== null && output.textContent === String(valorEsperado);
            },
            1,           // <-- argumento pasado al callback del navegador
            { timeout: 5000 }
        );

        // Verificar visualmente que el output muestra "1"
        await expect(formulario.valorExperiencia).toHaveText('1');

        await formulario.enviar();
        await page.waitForURL(/\/Registro\/Confirmacion\/\d+/, { timeout: 10000 });
        await expect(formulario.panelConfirmacion).toBeVisible();

        console.log('✅ Slider mínimo (1) aceptado y mostrado correctamente');
    });

    test('Debe aceptar y mostrar el valor máximo del slider (100)', async ({ page }) => {
        const formulario = new FormularioPage(page);
        const datos = datosValidosBase({ nivel_experiencia: 100 });

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos);

        // waitForFunction con valor 100
        await page.waitForFunction(
            (valorEsperado) => {
                const output = document.getElementById('valorExperiencia');
                return output !== null && output.textContent === String(valorEsperado);
            },
            100,
            { timeout: 5000 }
        );

        await expect(formulario.valorExperiencia).toHaveText('100');

        await formulario.enviar();
        await page.waitForURL(/\/Registro\/Confirmacion\/\d+/, { timeout: 10000 });
        await expect(formulario.panelConfirmacion).toBeVisible();

        console.log('✅ Slider máximo (100) aceptado y mostrado correctamente');
    });

});
