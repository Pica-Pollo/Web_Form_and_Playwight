// @ts-check
const { test, expect } = require('@playwright/test');
const { FormularioPage } = require('../pages/FormularioPage');
const { datosValidosBase } = require('../utils/helpers');

/**
 * CASO DE PRUEBA 4: Edad fuera del rango permitido
 * --------------------------------------------------
 * Objetivo: el Modelo define [Range(18, 99)] para Edad. Probamos
 * dos valores fuera de rango: 5 (muy joven) y 150 (imposible).
 *
 * ESTRATEGIA DE ESPERA DEMOSTRADA: page.waitForLoadState('networkidle')
 * ¿Por qué? Tras un submit fallido, el servidor re-renderiza la página
 * completa (no es una navegación SPA). waitForLoadState('networkidle')
 * espera a que no haya peticiones de red activas, asegurando que toda
 * la página (incluyendo CSS/JS) terminó de cargar antes de inspeccionarla.
 */
test.describe('Caso 4: Edad fuera de rango', () => {

    test('Debe rechazar una edad menor al mínimo permitido (5 años)', async ({ page }) => {
        const formulario = new FormularioPage(page);
        const datos = datosValidosBase({ edad: 5 });

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos);
        await formulario.enviar();

        // ESPERA EXPLÍCITA POR ESTADO DE CARGA DE PÁGINA
        await expect(formulario.alertaValidacion).toBeVisible();

        await expect(formulario.alertaValidacion).toBeVisible();
        await expect(page).not.toHaveURL(/Confirmacion/);
    });

    test('Debe rechazar una edad mayor al máximo permitido (150 años)', async ({ page }) => {
        const formulario = new FormularioPage(page);
        const datos = datosValidosBase({ edad: 150 });

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos);
        await formulario.enviar();

        await expect(formulario.alertaValidacion).toBeVisible();

        await expect(formulario.alertaValidacion).toBeVisible();
        await expect(page).not.toHaveURL(/Confirmacion/);
    });

});
