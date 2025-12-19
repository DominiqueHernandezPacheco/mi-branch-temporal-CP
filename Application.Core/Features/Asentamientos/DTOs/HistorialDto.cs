using System.Collections.Generic;

namespace Application.Core.Features.Asentamientos.DTOs
{
    public class HistorialDto
    {
        public long IdLog { get; set; }
        public string Accion { get; set; }  // El texto crudo
        public string Usuario { get; set; }
        public string Fecha { get; set; }
        public string? DatosAnterioresJson { get; set; }

        // --- NUEVO: LISTA ESTRUCTURADA PARA EL HOVER ---
        public List<CambioDetalleDto> CambiosDetectados { get; set; } = new List<CambioDetalleDto>();
    }

    // Clase hija para el detalle
    public class CambioDetalleDto
    {
        public string Campo { get; set; }         // Ej: "Nombre"
        public string ValorAnterior { get; set; } // Ej: "San Francisco"
        public string ValorNuevo { get; set; }    // Ej: "San Pancho"
    }
}