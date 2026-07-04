// @ts-check
const XLSX = require('xlsx');
const path = require('path');

/**
 * excelReader.js
 *
 * Lee el archivo Excel (data/registros.xlsx) y devuelve un arreglo de
 * objetos JavaScript, uno por cada fila/registro. Las claves del objeto
 * son los nombres de columna en minúsculas con guión bajo (snake_case),
 * que es justo lo que espera FormularioPage.llenarFormularioCompleto().
 *
 * @param {string} [rutaArchivo] - ruta al .xlsx (por defecto: ./data/registros.xlsx)
 * @returns {Array<object>} arreglo de registros
 */
function leerRegistrosExcel(rutaArchivo) {
    const ruta = rutaArchivo
        ? path.resolve(rutaArchivo)
        : path.resolve(__dirname, '../data/registros.xlsx');

    const workbook = XLSX.readFile(ruta);
    const nombreHoja = workbook.SheetNames[0];
    const hoja = workbook.Sheets[nombreHoja];

    // defval: '' evita que celdas vacías generen "undefined"
    const registros = XLSX.utils.sheet_to_json(hoja, { defval: '' });

    return registros.map(normalizarRegistro);
}

/**
 * Normaliza tipos de datos que vienen "crudos" desde Excel
 * (por ejemplo, fechas como número de serie o como string).
 */
function normalizarRegistro(fila) {
    return {
        nombre: String(fila.nombre ?? '').trim(),
        apellido: String(fila.apellido ?? '').trim(),
        email: String(fila.email ?? '').trim(),
        password: String(fila.password ?? '').trim(),
        edad: Number(fila.edad ?? 0),
        fecha_nacimiento: normalizarFecha(fila.fecha_nacimiento),
        hora_preferida: String(fila.hora_preferida ?? '').trim(),
        biografia: String(fila.biografia ?? '').trim(),
        genero: String(fila.genero ?? '').trim(),
        pais: String(fila.pais ?? '').trim(),
        plataforma_favorita: String(fila.plataforma_favorita ?? '').trim(),
        acepta_terminos: String(fila.acepta_terminos ?? 'SI').trim().toUpperCase(),
        recibir_notificaciones: String(fila.recibir_notificaciones ?? 'NO').trim().toUpperCase(),
        nivel_experiencia: Number(fila.nivel_experiencia ?? 50),
        color_favorito: String(fila.color_favorito ?? '#00F0FF').trim(),
        sonido_activado: String(fila.sonido_activado ?? 'SI').trim().toUpperCase(),
    };
}

/**
 * Excel a veces entrega fechas como número de serie (ej: 33000) en vez
 * de texto. Esta función normaliza a formato YYYY-MM-DD que es lo que
 * espera <input type="date">.
 */
function normalizarFecha(valor) {
    if (!valor) return '';

    // Si ya viene como texto "1990-05-15", lo dejamos igual
    if (typeof valor === 'string' && /^\d{4}-\d{2}-\d{2}$/.test(valor)) {
        return valor;
    }

    // Si viene como número de serie de Excel, lo convertimos
    if (typeof valor === 'number') {
        const fecha = XLSX.SSF.parse_date_code(valor);
        const mm = String(fecha.m).padStart(2, '0');
        const dd = String(fecha.d).padStart(2, '0');
        return `${fecha.y}-${mm}-${dd}`;
    }

    // Si viene como objeto Date
    if (valor instanceof Date) {
        const yyyy = valor.getFullYear();
        const mm = String(valor.getMonth() + 1).padStart(2, '0');
        const dd = String(valor.getDate()).padStart(2, '0');
        return `${yyyy}-${mm}-${dd}`;
    }

    return String(valor);
}

module.exports = { leerRegistrosExcel };
