// @ts-check

/**
 * FormularioPage.js
 *
 * PAGE OBJECT MODEL (POM) del formulario de registro.
 *
 * ¿Por qué usamos este patrón?
 * En lugar de escribir page.locator('#Nombre') repetido en cada test,
 * centralizamos TODOS los selectores aquí. Si el formulario cambia un
 * id o una clase, solo corregimos este archivo y no los 12 tests.
 *
 * ESTRATEGIAS DE SELECTORES (a propósito usamos varias, NO solo IDs):
 *   - Por ID                  → '#Nombre'
 *   - Por atributo [name]     → 'input[name="Apellido"]'
 *   - Por clase                → '.input-email'
 *   - Por atributo [data-*]   → '[data-testid="input-password"]'
 *   - Por atributo [type]     → 'input[type="date"]'
 *   - Por placeholder          → 'input[placeholder="jugador@correo.com"]'
 *   - Selectores compuestos    → '.form-actions .btn--primary'
 *   - :first-child / :last-child / :nth-child → '.form-actions .btn:first-child'
 */
class FormularioPage {
    /**
     * @param {import('@playwright/test').Page} page
     */
    constructor(page) {
        this.page = page;

        // ---------------- Sección 1: Datos básicos ----------------
        this.inputNombre = page.locator('#Nombre');                              // selector por ID
        this.inputApellido = page.locator('input[name="Apellido"]');             // selector por atributo [name]
        this.inputEmail = page.locator('.input-email');                          // selector por clase
        this.inputPassword = page.locator('[data-testid="input-password"]');     // selector por data-attribute

        // ---------------- Sección 2: Perfil ----------------
        this.inputEdad = page.locator('#Edad');                                  // ID
        this.inputFechaNacimiento = page.locator('input[type="date"]');          // atributo [type]
        this.inputHoraPreferida = page.locator('input[name="HoraPreferida"]');   // atributo [name]
        this.textareaBiografia = page.locator('textarea[name="Biografia"]');     // selector descendiente + atributo
        // Radio buttons: función que arma el selector dinámicamente por [value]
        this.radioGenero = (valor) => page.locator(`input[name="Genero"][value="${valor}"]`);

        // ---------------- Sección 3: Preferencias ----------------
        this.selectPais = page.locator('#selectPais');                          // ID
        this.selectPlataforma = page.locator('select[name="PlataformaFavorita"]'); // atributo [name]
        this.sliderExperiencia = page.locator('[data-testid="slider-experiencia"]'); // data-attribute
        this.valorExperiencia = page.locator('#valorExperiencia');
        this.inputColor = page.locator('#colorFavorito');                       // ID
        this.inputAvatar = page.locator('input[name="AvatarArchivo"]');         // atributo [name]
        this.nombreArchivoMostrado = page.locator('#nombreArchivo');

        // ---------------- Sección 4: Confirmación ----------------
        this.botonProbarSonido = page.locator('[data-testid="btn-probar-sonido"]');
        this.toggleSonido = page.locator('#sonidoActivado');
        this.toggleNotificaciones = page.locator('#recibirNotificaciones');
        this.checkboxTerminos = page.locator('#aceptaTerminos');

        // ---------------- Botones (selectores compuestos + posición) ----------------
        this.botonLimpiar = page.locator('.form-actions .btn:first-child');     // :first-child
        this.botonCancelar = page.locator('.form-actions a.btn:nth-child(2)');  // :nth-child
        this.botonGuardar = page.locator('.form-actions .btn--primary:last-child'); // compuesto + :last-child

        // ---------------- Mensajes / Confirmación ----------------
        this.alertaError = page.locator('[data-testid="alerta-error"]');
        this.alertaValidacion = page.locator('[data-testid="alerta-validacion"]');
        this.panelConfirmacion = page.locator('[data-testid="panel-confirmacion"]');
        this.resumenId = page.locator('[data-testid="resumen-id"]');
        this.resumenEmail = page.locator('[data-testid="resumen-email"]');

        // ---------------- Errores de campo individuales (asp-validation-for) ----------------
        this.errorEdad = page.locator('span[data-valmsg-for="Edad"]');
        this.errorEmail = page.locator('span[data-valmsg-for="Email"]');
        this.errorFecha = page.locator('span[data-valmsg-for="FechaNacimiento"]');
        this.errorAvatar = page.locator('span[data-valmsg-for="AvatarArchivo"]');
        this.errorTerminos = page.locator('span[data-valmsg-for="AceptaTerminos"]');
        this.errorGenero = page.locator('span[data-valmsg-for="Genero"]');
    }

    /**
     * Navega a la página principal del formulario.
     * Usa goto() + espera automática de Playwright a que el DOM esté listo.
     */
        async ir() {
            // FIX: Eliminar waitForLoadState('networkidle').
            // goto() ya espera el evento 'load' por defecto de manera confiable.
            await this.page.goto('/Registro/Index');
        }

    /**
     * Llena TODOS los controles del formulario con los datos provistos.
     * @param {object} datos - objeto con las propiedades del registro (ver excelReader.js)
     * @param {string} [rutaImagen] - ruta absoluta a una imagen para el campo Avatar (opcional)
     */
    async llenarFormularioCompleto(datos, rutaImagen = null) {
        // --- TextBox ---
        await this.inputNombre.fill(datos.nombre ?? '');
        await this.inputApellido.fill(datos.apellido ?? '');

        // --- Email ---
        await this.inputEmail.fill(datos.email ?? '');

        // --- Password ---
        await this.inputPassword.fill(datos.password ?? '');

        // --- Number ---
        await this.inputEdad.fill(String(datos.edad ?? ''));

        // --- Date Picker --- (formato esperado: YYYY-MM-DD)
        if (datos.fecha_nacimiento) {
            await this.inputFechaNacimiento.fill(datos.fecha_nacimiento);
        }

        // --- Time Picker --- (formato esperado: HH:MM)
        if (datos.hora_preferida) {
            await this.inputHoraPreferida.fill(datos.hora_preferida);
        }

        // --- TextArea ---
        if (datos.biografia) {
            await this.textareaBiografia.fill(datos.biografia);
        }

        // --- Radio Button ---
        if (datos.genero) {
            await this.radioGenero(datos.genero).evaluate(node => {
                if (!node.checked) node.click();
            });
        }

        // --- Select: País ---
        if (datos.pais) {
            await this.selectPais.selectOption(datos.pais);
        }

        // --- Select: Plataforma ---
        if (datos.plataforma_favorita) {
            await this.selectPlataforma.selectOption(datos.plataforma_favorita);
        }

        // --- Range Slider --- (se rellena con fill, dispara el evento 'input' vía JS)
        if (datos.nivel_experiencia !== undefined && datos.nivel_experiencia !== null) {
            await this.sliderExperiencia.fill(String(datos.nivel_experiencia));
        }

        // --- Color Picker ---
        if (datos.color_favorito) {
            await this.inputColor.fill(datos.color_favorito);
        }

        // --- File Upload ---
        if (rutaImagen) {
            await this.inputAvatar.setInputFiles(rutaImagen);
        }

// --- Switch: Notificaciones ---
        if (datos.recibir_notificaciones === 'SI') {
            // Evaluamos directamente en el DOM para saltarnos cualquier restricción visual de CSS
            await this.toggleNotificaciones.evaluate(node => {
                if (!node.checked) node.click();
            });
        }

        // --- Audio Control (toggle de sonido) ---
        if (datos.sonido_activado === 'NO') {
            await this.toggleSonido.evaluate(node => {
                if (node.checked) node.click();
            });
        }

        // --- CheckBox: Términos ---
        if (datos.acepta_terminos === 'SI') {
            await this.checkboxTerminos.evaluate(node => {
                if (!node.checked) node.click();
            });
        }
    }

    /**
     * Click en el botón "Guardar Personaje" (submit).
     */
/**
     * Click en el botón "Guardar Personaje" (submit).
     */
    async enviar() {
        // Al agregar noWaitAfter: true, Playwright no se quedará colgado esperando la red
        await this.botonGuardar.click({ noWaitAfter: true });
    }
    /**
     * Click en "Limpiar" (reset del formulario).
     */
    async limpiar() {
        await this.botonLimpiar.click();
    }
}

module.exports = { FormularioPage };
