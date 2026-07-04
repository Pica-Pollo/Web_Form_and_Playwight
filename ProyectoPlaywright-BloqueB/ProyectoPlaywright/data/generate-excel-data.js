// @ts-check
const XLSX = require('xlsx');
const path = require('path');

/**
 * generate-excel-data.js
 *
 * Genera el archivo data/registros.xlsx con datos de ejemplo.
 * Cada columna corresponde EXACTAMENTE a un campo del formulario web.
 *
 * Uso:
 *   node data/generate-excel-data.js
 *   (o: npm run generar-excel)
 */

const nombres = ['Akira', 'Yuna', 'Kazuki', 'Mei', 'Ren', 'Hana', 'Sora', 'Aiko', 'Haru', 'Nico', 'Valentina', 'Mateo', 'Sofia', 'Diego', 'Camila', 'Lucas', 'Emma', 'Leo', 'Mia', 'Thiago'];
const apellidos = ['Tanaka', 'Suzuki', 'Yamamoto', 'Takahashi', 'Ito', 'Watanabe', 'Nakamura', 'Kobayashi', 'García', 'Rodríguez', 'Martínez', 'López', 'Fernández', 'González', 'Pérez', 'Sánchez', 'Díaz', 'Torres', 'Ramírez', 'Flores'];
const paises = ['México', 'Colombia', 'Argentina', 'Chile', 'España', 'Perú', 'Estados Unidos', 'Japón'];
const plataformas = ['PC', 'PlayStation', 'Xbox', 'Nintendo Switch', 'Móvil'];
const generos = ['Masculino', 'Femenino', 'Otro'];
const biografias = [
    'Jugador competitivo desde los 12 años.',
    'Amante de los RPG y las historias profundas.',
    'Streamer en mis tiempos libres.',
    'Coleccionista de logros y trofeos.',
    'Prefiero los juegos cooperativos.',
    'Speedrunner aficionado.',
    'Disfruto los juegos de estrategia por turnos.',
    ''
];
const colores = ['#00F0FF', '#FF2BD6', '#7C3AED', '#39FF88', '#FFD500', '#FF4757', '#5AF8FF'];

function aleatorio(arr) { return arr[Math.floor(Math.random() * arr.length)]; }

function generarPassword() {
    const may = 'ABCDEFGHJKLMNPQRSTUVWXYZ';
    const min = 'abcdefghijkmnpqrstuvwxyz';
    const num = '23456789';
    const esp = '@$!%*?&';
    let pass = aleatorio(may.split('')) + aleatorio(min.split('')) + aleatorio(num.split('')) + aleatorio(esp.split(''));
    for (let i = 0; i < 5; i++) {
        pass += aleatorio((may + min + num).split(''));
    }
    return pass.split('').sort(() => 0.5 - Math.random()).join('');
}

function generarFechaNacimiento() {
    const hoy = new Date();
    const edad = Math.floor(Math.random() * 40) + 18; // 18 a 57 años
    const anio = hoy.getFullYear() - edad;
    const mes = String(Math.floor(Math.random() * 12) + 1).padStart(2, '0');
    const dia = String(Math.floor(Math.random() * 28) + 1).padStart(2, '0');
    return { fecha: `${anio}-${mes}-${dia}`, edad };
}

function generarHora() {
    const h = String(Math.floor(Math.random() * 24)).padStart(2, '0');
    const m = aleatorio(['00', '15', '30', '45']);
    return `${h}:${m}`;
}

function generarRegistro(indice) {
    const nombre = aleatorio(nombres);
    const apellido = aleatorio(apellidos);
    const { fecha, edad } = generarFechaNacimiento();

    return {
        nombre,
        apellido,
        email: `${nombre.toLowerCase()}.${apellido.toLowerCase()}${indice}@correojugador.com`,
        password: generarPassword(),
        edad,
        fecha_nacimiento: fecha,
        hora_preferida: generarHora(),
        biografia: aleatorio(biografias),
        genero: aleatorio(generos),
        pais: aleatorio(paises),
        plataforma_favorita: aleatorio(plataformas),
        acepta_terminos: 'SI',
        recibir_notificaciones: Math.random() > 0.5 ? 'SI' : 'NO',
        nivel_experiencia: Math.floor(Math.random() * 100) + 1,
        color_favorito: aleatorio(colores),
        sonido_activado: Math.random() > 0.2 ? 'SI' : 'NO',
    };
}

function crearExcel(cantidad = 20) {
    const registros = [];
    for (let i = 1; i <= cantidad; i++) {
        registros.push(generarRegistro(i));
    }

    const columnas = [
        'nombre', 'apellido', 'email', 'password', 'edad', 'fecha_nacimiento',
        'hora_preferida', 'biografia', 'genero', 'pais', 'plataforma_favorita',
        'acepta_terminos', 'recibir_notificaciones', 'nivel_experiencia',
        'color_favorito', 'sonido_activado'
    ];

    const wb = XLSX.utils.book_new();
    const ws = XLSX.utils.json_to_sheet(registros, { header: columnas });

    ws['!cols'] = columnas.map(c => ({ wch: Math.max(c.length + 2, 16) }));

    XLSX.utils.book_append_sheet(wb, ws, 'Registros');

    const outputPath = path.join(__dirname, 'registros.xlsx');
    XLSX.writeFile(wb, outputPath);

    console.log(`✅ Archivo generado: ${outputPath}`);
    console.log(`📊 Total de registros: ${registros.length}`);
    console.log(`📋 Columnas: ${columnas.join(', ')}`);
}

// Permite indicar cuántos registros generar: node generate-excel-data.js 50
const cantidadArg = parseInt(process.argv[2], 10);
crearExcel(Number.isFinite(cantidadArg) && cantidadArg > 0 ? cantidadArg : 20);
