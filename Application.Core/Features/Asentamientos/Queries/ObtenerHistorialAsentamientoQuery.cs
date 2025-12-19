using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Core.Infraestructure.Persistance;
using Application.Core.Features.Asentamientos.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace Application.Core.Features.Asentamientos.Queries
{
    // 1. EL QUERY
    public class ObtenerHistorialAsentamientoQuery : IRequest<List<HistorialDto>>
    {
        public int IdAsentamiento { get; set; }

        public ObtenerHistorialAsentamientoQuery(int id)
        {
            IdAsentamiento = id;
        }
    }

    // 2. EL HANDLER
    public class ObtenerHistorialAsentamientoQueryHandler : IRequestHandler<ObtenerHistorialAsentamientoQuery, List<HistorialDto>>
    {
        private readonly LoginDefaultDbContext _context;

        public ObtenerHistorialAsentamientoQueryHandler(LoginDefaultDbContext context)
        {
            _context = context;
        }

        public async Task<List<HistorialDto>> Handle(ObtenerHistorialAsentamientoQuery request, CancellationToken cancellationToken)
        {
            // PASO 1: Consulta a Base de Datos (Traemos datos crudos)
            // Usamos una proyección anónima (select new { ... }) para eficiencia
            var logsCrudos = await (from log in _context.SysRegistroModificaciones
                                    
                                    // Join con Usuario
                                    join user in _context.Usuarios on log.FkUsuario equals user.IdUsuario
                                    
                                    // Left Join con JSON
                                    join json in _context.Set<Domain.Entities.SysRegistroModificacionesJson>() 
                                        on log.IdLog equals json.IdLog into jsonGroup
                                    from subJson in jsonGroup.DefaultIfEmpty()

                                    where log.FkAsentamiento == request.IdAsentamiento
                                    orderby log.FechaHora descending

                                    // Seleccionamos solo lo necesario para procesar en memoria
                                    select new 
                                    {
                                        log.IdLog,
                                        log.Accion,
                                        Usuario = user.NombreUsuario,
                                        log.FechaHora,
                                        DatosAnterioresJson = subJson != null ? subJson.DatosAnteriores : null
                                    }).ToListAsync(cancellationToken);

            // PASO 2: Procesamiento en Memoria (Parsear texto para el Hover)
            var historialFinal = logsCrudos.Select(item => new HistorialDto
            {
                IdLog = item.IdLog,
                Accion = item.Accion, // Texto completo para referencia
                Usuario = item.Usuario,
                
                // Formato de fecha seguro
                Fecha = item.FechaHora.HasValue 
                        ? item.FechaHora.Value.ToString("dd/MM/yyyy HH:mm") 
                        : "Sin Fecha",

                DatosAnterioresJson = item.DatosAnterioresJson,

                // AQUÍ LA MAGIA: Convertimos el texto "A -> B" en una lista limpia
                CambiosDetectados = ParsearCambios(item.Accion)

            }).ToList();

            return historialFinal;
        }

       // --- MÉTODO AUXILIAR MEJORADO ---
        private List<CambioDetalleDto> ParsearCambios(string accion)
        {
            var lista = new List<CambioDetalleDto>();

            // Validaciones básicas
            if (string.IsNullOrEmpty(accion) || !accion.StartsWith("EDITAR")) 
                return lista; 

            // Limpiamos el prefijo. Quitamos "EDITAR: " o "EDITAR (JSON)"
            var textoLimpio = accion
                .Replace("EDITAR: ", "")
                .Replace("EDITAR (JSON)", "") // Por si acaso quedo alguno viejo
                .Trim();

            if (string.IsNullOrEmpty(textoLimpio)) return lista;

            // Separamos por la barrita " | "
            var partes = textoLimpio.Split('|');

            foreach (var parte in partes)
            {
                var segmentoLimpio = parte.Trim();
                if (string.IsNullOrEmpty(segmentoLimpio)) continue;

                // CASO 1: Formato detallado "Nombre: 'A' -> 'B'"
                if (segmentoLimpio.Contains("->"))
                {
                    var segmentos = segmentoLimpio.Split("->");
                    var izquierda = segmentos[0].Trim(); 
                    var valorNuevo = segmentos[1].Trim().Replace("'", ""); 

                    if (izquierda.Contains(":"))
                    {
                        var datosCampo = izquierda.Split(':');
                        lista.Add(new CambioDetalleDto
                        {
                            Campo = datosCampo[0].Trim(),
                            ValorAnterior = datosCampo[1].Trim().Replace("'", ""),
                            ValorNuevo = valorNuevo
                        });
                    }
                    else
                    {
                        // Caso raro "A -> B" sin nombre de campo
                        lista.Add(new CambioDetalleDto { Campo = "General", ValorAnterior = izquierda, ValorNuevo = valorNuevo });
                    }
                }
                // CASO 2: Formato simple "Tipo cambió" (AQUÍ ESTÁ EL ARREGLO PARA TUS LOGS 8 y 9)
                else
                {
                    lista.Add(new CambioDetalleDto
                    {
                        Campo = segmentoLimpio, // Pondrá "Tipo cambió"
                        ValorAnterior = "-",
                        ValorNuevo = "Modificado" // Indicador genérico
                    });
                }
            }
            return lista;
        }
        
    }
}