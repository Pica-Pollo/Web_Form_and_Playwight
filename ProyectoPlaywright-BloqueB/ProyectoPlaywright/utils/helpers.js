// @ts-check

/**
 * helpers.js
 * Funciones auxiliares reutilizadas por varios archivos de test.
 */

/**
 * Genera un email único usando timestamp + número aleatorio.
 * Evita el error "El correo ya está registrado" al re-ejecutar los tests
 * varias veces contra la misma base de datos.
 */
function generarEmailUnico(prefijo = 'jugador') {
    const timestamp = Date.now();
    const random = Math.floor(Math.random() * 10000);
    return `${prefijo}.${timestamp}.${random}@correojugador.com`;
}

/**
 * Datos base válidos para un registro. Los tests pueden sobreescribir
 * solo el/los campos que les interesa poner inválidos.
 */
function datosValidosBase(overrides = {}) {
    return {
        nombre: 'Akira',
        apellido: 'Tanaka',
        email: generarEmailUnico(),
        password: 'Pass123!@',
        edad: 25,
        fecha_nacimiento: '1999-05-15',
        hora_preferida: '20:00',
        biografia: 'Jugador de prueba generado por Playwright.',
        genero: 'Masculino',
        pais: 'México',
        plataforma_favorita: 'PC',
        acepta_terminos: 'SI',
        recibir_notificaciones: 'SI',
        nivel_experiencia: 75,
        color_favorito: '#00F0FF',
        sonido_activado: 'NO', // NO para que los tests no dependan de audio real
        ...overrides,
    };
}

/**
 * Consulta el endpoint JSON de la app ASP.NET (Registro/ApiUsuarioPorEmail)
 * para verificar, DESDE CÓDIGO, que el registro quedó guardado en SQL Server.
 * Esto evita tener que abrir SSMS manualmente para validar cada test.
 *
 * @param {import('@playwright/test').APIRequestContext} request
 * @param {string} baseURL
 * @param {string} email
 */
async function verificarUsuarioEnBD(request, baseURL, email) {
    const respuesta = await request.get(`${baseURL}/Registro/ApiUsuarioPorEmail`, {
        params: { email },
    });
    const json = await respuesta.json();
    return json;
}

module.exports = { generarEmailUnico, datosValidosBase, verificarUsuarioEnBD };
