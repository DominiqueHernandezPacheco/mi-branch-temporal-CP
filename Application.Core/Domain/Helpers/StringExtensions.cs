using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using HashidsNet;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Application.Core.Domain.Helpers
{
    public static class StringExtensions
    {
        

        private static IConfiguration _config;
        public static void Configure(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Metodo que permite convertir un objeto a un string en formato JSON
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EncodeBase64(this string value)
        {
            var valueBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(valueBytes);
        }

        /// <summary>
        /// Método de extensión para decodificar un string en base64.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string DecodeBase64(this string value)
        {
            var base64Encoded = value.Split('.')[1];
            var mod4 = base64Encoded.Length % 4;

            if (mod4 > 0)
            {
                base64Encoded += new string('=', 4 - mod4);
            }

            byte[] data = Convert.FromBase64String(base64Encoded);
            return ASCIIEncoding.ASCII.GetString(data);
        }

        /// <summary>
        /// Método de extensión para desearilizar el json string.
        /// </summary>
        /// <param name="value"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns>Objeto especificado</returns>
        public static T? DeserializeObject<T>(this string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static string ToHashId(this int number) =>
            GetHasher().Encode(number);

        public static int FromHashId(this string encoded) =>
            GetHasher().Decode(encoded).FirstOrDefault();

        private static Hashids GetHasher() => new(_config["HashIdSalt"], 16);

        /// <summary>
        /// Indentifica el tipo de fecha que puede contener un texto.
        /// </summary>
        /// <param name="input">Texto que pueda ser una fecha</param>
        /// <returns>
        /// <b>La fecha convertida con el formato y el número de identificación que le corresponde:</b>
        /// <br>1: Fecha completa con hora (yyyy-MM-dd HH:mm:ss)</br>
        /// <br>2: Fecha completa sin hora (dd/MM/yyyy)</br>
        /// <br>3: Día y mes (dd/MM)</br>
        /// <br>4: Solo día (dd)</br>
        /// <br>5: Solo mes (MM)</br>
        /// <br>6: Solo año (yyyy)</br>
        /// 
        /// </returns>
        public static (DateTime? fecha, int tipoFecha) IdentifyDateType(this string input)
        {
            input = input.StandardizeDate(); // Normaliza el input antes de procesarlo

            if (DateTime.TryParseExact(input, "dd-MM-yyyy HH:mm:ss", new CultureInfo("es-MX"), DateTimeStyles.None, out var fechaCompleta))
                return (fechaCompleta, 1); // Fecha completa con hora
            if (DateTime.TryParseExact(input, "dd/MM/yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out var fechaConDiaMesAnio))
                return (fechaConDiaMesAnio, 2); // Fecha sin hora
            if (DateTime.TryParseExact(input, "dd/MM", new CultureInfo("es-MX"), DateTimeStyles.None, out var fechaDiaMes))
                return (fechaDiaMes, 3); // Día y mes
            if (DateTime.TryParseExact(input, "dd", new CultureInfo("es-MX"), DateTimeStyles.None, out var fechaSoloDia))
                return (fechaSoloDia, 4); // Solo día
            if (DateTime.TryParseExact(input, "MM", new CultureInfo("es-MX"), DateTimeStyles.None, out var fechaSoloMes))
                return (fechaSoloMes, 5); // Solo mes
            if (DateTime.TryParseExact(input, "yyyy", new CultureInfo("es-MX"), DateTimeStyles.None, out var fechaSoloAnio))
                return (fechaSoloAnio, 6); // Solo año

            return (null, 0); // No es una fecha válida
        }

        /// <summary>
        /// Filtra y limpia la fecha posible fecha ingresada para devolver alguno de los formtado del método IdentifyDateType
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string StandardizeDate(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            // Divide el input en partes separadas por "/"
            var partes = input.Split('/', StringSplitOptions.RemoveEmptyEntries);

            string dia = string.Empty, mes = string.Empty, año = string.Empty;

            // Procesa cada parte según su posición y corrige valores parciales
            if (partes.Length > 0)
            {
                dia = partes[0].Trim(); // Toma el día como está
                if (!int.TryParse(dia, out _) || dia == "0") dia = string.Empty; // Descarta valores inválidos
            }

            if (partes.Length > 1)
            {
                mes = partes[1].Trim(); // Toma el mes como está
                if (!int.TryParse(mes, out _) || mes == "0") mes = string.Empty; // Descarta valores inválidos
            }

            if (partes.Length > 2)
            {
                año = partes[2].Trim(); // Toma el año como está
                if (!int.TryParse(año, out _) || año.Length < 4) año = string.Empty; // Solo acepta años con 4 dígitos
            }

            // Reconstruye la fecha normalizada
            if (!string.IsNullOrEmpty(dia) && !string.IsNullOrEmpty(mes) && !string.IsNullOrEmpty(año))
                return $"{dia}/{mes}/{año}";
            if (!string.IsNullOrEmpty(dia) && !string.IsNullOrEmpty(mes))
                return $"{dia}/{mes}";
            if (!string.IsNullOrEmpty(dia))
                return dia;

            return string.Empty;
        }
    }
    

}