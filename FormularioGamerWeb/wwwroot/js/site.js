/**
 * site.js
 * JavaScript de cliente para el Formulario Gamer.
 * Solo maneja interactividad de UI (no hace validación de negocio;
 * esa vive en el servidor, en RegistroController + Data Annotations).
 */

window.GamerForm = (function () {

    /**
     * Actualiza el texto que muestra el valor numérico del slider
     * de "Nivel de Experiencia" en tiempo real.
     */
    function initRangeSlider() {
        const slider = document.getElementById('rangoExperiencia');
        const salida = document.getElementById('valorExperiencia');
        if (!slider || !salida) return;

        const actualizarFondo = (el) => {
            const min = Number(el.min) || 0;
            const max = Number(el.max) || 100;
            const pct = ((el.value - min) / (max - min)) * 100;
            el.style.background = `linear-gradient(to right, var(--cyan) ${pct}%, var(--line) ${pct}%)`;
        };

        slider.addEventListener('input', () => {
            salida.textContent = slider.value;
            actualizarFondo(slider);
        });

        actualizarFondo(slider);
    }

    /**
     * Sincroniza el texto que muestra el código HEX del color seleccionado.
     */
    function initColorPicker() {
        const color = document.getElementById('colorFavorito');
        const texto = document.getElementById('colorValorTexto');
        if (!color || !texto) return;

        color.addEventListener('input', () => {
            texto.textContent = color.value.toUpperCase();
        });
    }

    /**
     * Muestra el nombre del archivo seleccionado en el Upload de Avatar.
     */
    function initFileUpload() {
        const input = document.getElementById('avatarArchivo');
        const nombre = document.getElementById('nombreArchivo');
        if (!input || !nombre) return;

        input.addEventListener('change', () => {
            if (input.files && input.files.length > 0) {
                const file = input.files[0];
                const maxBytes = 5 * 1024 * 1024;

                if (file.size > maxBytes) {
                    nombre.textContent = '⚠ Archivo muy grande (máx 5MB)';
                    nombre.style.color = 'var(--danger)';
                    input.value = '';
                    return;
                }

                nombre.textContent = `✓ ${file.name}`;
                nombre.style.color = 'var(--success)';
            } else {
                nombre.textContent = 'Ningún archivo seleccionado';
                nombre.style.color = '';
            }
        });
    }

    /**
     * Control de Audio: genera un "beep" tipo retro/arcade con la Web Audio API
     * (no depende de ningún archivo de audio externo, evita temas de licencias).
     * Se reproduce al pulsar "Probar Sonido" o al enviar el formulario,
     * solo si el toggle "Sonido Activado" está encendido.
     */
    function reproducirBeep() {
        try {
            const AudioCtx = window.AudioContext || window.webkitAudioContext;
            const ctx = new AudioCtx();
            const osc = ctx.createOscillator();
            const gain = ctx.createGain();

            osc.type = 'square';
            osc.frequency.setValueAtTime(660, ctx.currentTime);
            osc.frequency.exponentialRampToValueAtTime(990, ctx.currentTime + 0.12);

            gain.gain.setValueAtTime(0.08, ctx.currentTime);
            gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.18);

            osc.connect(gain);
            gain.connect(ctx.destination);

            osc.start();
            osc.stop(ctx.currentTime + 0.2);
        } catch (e) {
            // Silenciosamente ignorar si el navegador bloquea audio sin interacción previa
            console.warn('No se pudo reproducir el efecto de sonido:', e);
        }
    }

    function initAudioControl() {
        const boton = document.getElementById('botonProbarSonido');
        const toggle = document.getElementById('sonidoActivado');

        if (boton) {
            boton.addEventListener('click', () => {
                if (!toggle || toggle.checked) {
                    reproducirBeep();
                }
            });
        }

        const formulario = document.getElementById('formularioRegistro');
        if (formulario) {
            formulario.addEventListener('submit', () => {
                if (toggle && toggle.checked) {
                    reproducirBeep();
                }
            });
        }
    }

    /**
     * Auto-cierre de la alerta de error tras unos segundos.
     */
    function initAlertaAutoCierre() {
        const alerta = document.getElementById('alertaError');
        if (!alerta) return;

        setTimeout(() => {
            alerta.style.transition = 'opacity 400ms ease';
            alerta.style.opacity = '0';
            setTimeout(() => alerta.remove(), 400);
        }, 6000);
    }

    function init() {
        initRangeSlider();
        initColorPicker();
        initFileUpload();
        initAudioControl();
        initAlertaAutoCierre();
    }

    return { init };
})();

document.addEventListener('DOMContentLoaded', function () {
    window.GamerForm.init();
});