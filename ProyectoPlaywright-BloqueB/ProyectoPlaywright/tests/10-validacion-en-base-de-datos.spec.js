// @ts-check
const { test, expect } = require('@playwright/test');
const { FormularioPage } = require('../pages/FormularioPage');
const { datosValidosBase, verificarUsuarioEnBD } = require('../utils/helpers');
const path = require('path');

/**
 * CASO DE PRUEBA 10: Registro exitoso y validación en la base de datos
 * ------------------------------------------------------------------
 * Objetivo: este es el test de integración completo.
 * 1. Llena el formulario con TODOS los controles (incluyendo avatar).
 * 2. Envía el formulario.
 * 3. Confirma la redirección a la pantalla de confirmación.
 * 4. Consulta el endpoint JSON de la app ASP.NET
 * (/Registro/ApiUsuarioPorEmail) para verificar POR CÓDIGO
 * que el registro quedó guardado en SQL Server.
 */

test.describe('Caso 10: Registro completo con validación en BD', () => {

    test('Debe guardar el registro en SQL Server y confirmarlo vía API', async ({ page, request }) => {
        const formulario = new FormularioPage(page);
        const datos = datosValidosBase();
        
        // Ruta del avatar de pruebas
        const rutaAvatar = path.resolve(__dirname, '../data/images/avatar-sample.png');

        await formulario.ir();
        await formulario.llenarFormularioCompleto(datos, rutaAvatar);

        // 1. Enviamos el formulario y esperamos la redirección a Confirmacion.
        await Promise.all([
            page.waitForURL(/\/Registro\/Confirmacion\/\d+/, { timeout: 15000 }),
            formulario.enviar(),
        ]);

        await expect(formulario.panelConfirmacion).toBeVisible();

        // 5. Leer el email de la confirmación para usarlo en la validación de BD
        const emailMostrado = await formulario.resumenEmail.textContent();
        console.log(`📋 Email en confirmación: ${emailMostrado}`);

        // ── Validación en Base de Datos ──────────────────────────────────
        // Consultamos el endpoint JSON que expone el controlador ASP.NET.
        // Esto verifica SIN abrir SSMS que el dato llegó a SQL Server.
        const baseURL = process.env.BASE_URL || 'http://localhost:5180';
        const resultado = await verificarUsuarioEnBD(request, baseURL, emailMostrado?.trim() ?? datos.email);

        // Validaciones sobre la respuesta de la base de datos (resultado)
        expect(resultado.success).toBe(true);
        expect(resultado.data).toBeDefined();
        expect(resultado.data.email).toBe(emailMostrado?.trim() ?? datos.email);
        expect(resultado.data.nombre).toBe(datos.nombre);

        console.log(`✅ Registro verificado en SQL Server. ID: ${resultado.data.id}`);
        console.log(`   Nombre: ${resultado.data.nombre} ${resultado.data.apellido}`);
        console.log(`   País:   ${resultado.data.pais}`);
    });

});