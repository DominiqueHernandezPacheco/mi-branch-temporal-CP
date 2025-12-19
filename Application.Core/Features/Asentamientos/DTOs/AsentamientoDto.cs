namespace Application.Core.Features.Asentamientos.DTOs
{
    public class AsentamientoDto
    {
        public int Id { get; set; }
        public string CodigoPostal { get; set; }
        public string Nombre { get; set; }
        public string Oficina { get; set; }
        
        // --- NOMBRES (Para mostrar en la tabla) ---
        public string Ciudad { get; set; }
        public string Municipio { get; set; }
        public string Tipo { get; set; }
        public string Estatus { get; set; } // "Activo" o "Inactivo"

        // --- DATOS TÉCNICOS (Nuevos: Para llenar el Modal de Editar) ---
        public int IdMunicipio { get; set; }
        public int? IdCiudad { get; set; }
        public int IdTipoAsentamiento { get; set; }
        public bool Activo { get; set; } // true/false para tu Switch

        // Auditoría
        public DateTime? Fecha { get; set; }
        public string UsuarioModifico { get; set; }
    }
}